using DoAn4.DTOs.AuthenticationDTOs;
using DoAn4.Interfaces;
using DoAn4.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DoAn4.DTOs.UserDTO;
using System.Security.Authentication;
using System.Globalization;

namespace DoAn4.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;
        

        public AuthenticationService( IConfiguration configuration, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IAccessTokenRepository accessTokenRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _accessTokenRepository = accessTokenRepository;
           
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Người dùng không tồn tại" }
                };
            }

            else if (user.IsEmailVerified == false)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Người dùng chưa xác thực tài khoản " }
                };
            }

            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Mật khẩu sai" }
                };

            }

            var accessToken = GenerateAccessToken(user);

            var refreshToken = GenerateRefreshToken();
            
            //lấy giờ theo múi giờ đông dương GMT+7
            var gmt7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var refTokenExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshTokenExpirationDays")), gmt7);

            var accTokenExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:AccessTokenExpirationMinutes"));
            accTokenExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(accTokenExpiresAt, gmt7);
            var refresh = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                Token = refreshToken,
                UserId = user.UserId,
                ExpiresAt = refTokenExpiresAt,
                IsRevoked = false
            };
            await _refreshTokenRepository.AddRefreshTokenAsync(refresh);     

            var access = new AccessToken
            {
                AccessTokenId = Guid.NewGuid(),
                Token = accessToken,
                UserId = user.UserId,
                ExpiresAt = accTokenExpiresAt
            };
            await _accessTokenRepository.AddAccessTokenAsync(access);

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthenticationResult> RefreshAccesstokenTokenAsync(string refreshToken, string accessToken)
        {
            var validatedToken = GetPrincipalFromToken(accessToken);       
            // Kiểm tra token đã hết hạn hay chưa
            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Token không hợp lệ" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims
                    .Single(x => x.Type == JwtRegisteredClaimNames.Exp)
                    .Value);

            // Lấy múi giờ của Việt Nam
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Chuyển đổi thời gian từ UNIX timestamp sang múi giờ Việt Nam
            DateTime expiryDateTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix),
                timeZone
            );

            if (expiryDateTimeLocal > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
            {
                return new AuthenticationResult { Errors = new[] { "AccessToken vẫn chưa hết hạn" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(jti);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "RefreshToken không tồn tại" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiresAt)
            {
                return new AuthenticationResult { Errors = new[] { "RefreshToken đã hết hạn" } };
            }

            if (storedRefreshToken.IsRevoked)
            {
                return new AuthenticationResult { Errors = new[] { "RefreshToken đã bị thu hồi" } };
            }

            if (storedRefreshToken.Token != refreshToken)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            // Tạo mới access token và refresh token
            var user = await _userRepository.GetUserByIdAsync(storedRefreshToken.UserId);
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            var gmt7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var refTokenExpiresAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshTokenExpirationDays")), gmt7);

            // Cập nhật refresh token mới trong database
            var newRefresh = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.UserId,
                ExpiresAt = refTokenExpiresAt
            };
            await _refreshTokenRepository.AddRefreshTokenAsync(newRefresh);

            // Cập nhật trạng thái của refresh token cũ
            storedRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateRefreshTokenAsync(storedRefreshToken);

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> RegisterAsync(UserRegisterDTO request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new AuthenticationException("Email đã tồn tại");
            }

            else if(request.Password!= request.RePassword)
            {
                throw new AuthenticationException("Hai mật khẩu không trùng khớp");

            }
            CreatedPasswordHash(request.Password, out byte[] passwordHash, out byte[] salt);
            var formatCurentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                PasswordSalt = salt,
                PasswordHash = passwordHash,
                Fullname = request.Fullname,
                Avatar = Path.Combine("FileUploads", "DefaultImage", "defaultAvatar.png"),
                CoverPhoto = Path.Combine("FileUploads", "DefaultImage", "coverPhotoDefault.jpg"),
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                CreateAt = DateTime.ParseExact(formatCurentTime, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsEmailVerified = false
            };

            await _userRepository.CreateUserAsync(newUser);

            return true;
        }

        public async Task<bool> LogoutAsync(string refreshToken, string accessToken)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

            var storedAccessToken = await _accessTokenRepository.GetAccessTokenAsync(accessToken);

            if (storedRefreshToken != null)
            {
                await _refreshTokenRepository.DeleteRefreshTokenAsync(storedRefreshToken.Token);
            }

            else if (storedAccessToken != null)
            {
                await _accessTokenRepository.DeleteAccessTokenAsync(storedAccessToken.Token);
            }

            return storedRefreshToken != null || storedAccessToken != null;
        }

        public async Task<ReadIdUserFromToken> GetIdUserFromAccessToken(string token)
        {
            var claimsPrincipal = GetPrincipalFromToken(token);
            if (claimsPrincipal == null)
            {
                return null;
            }

            var claims = claimsPrincipal.Claims;

            var userIdString = claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var userName = claims.FirstOrDefault(c => c.Type == "Name")?.Value;

            var expiresAtUnixTime = long.Parse(claims.FirstOrDefault(c => c.Type == "exp")?.Value);

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAtUnixTime).ToLocalTime().DateTime;
            var currentUtcTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));


            if (currentUtcTime > expiresAt)
            {
                return null;
            }

            else if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return null;
            }
            else
            {
                return new ReadIdUserFromToken
                {
                    UserId = userId,
                    UserName = userName
                };
            }

        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"])),
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtToken))
                    return null;

                else if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return claimsPrincipal;
            }
            catch
            {
                // Return null if the token is not valid
                return null;
            }
        }

        private string GenerateAccessToken(User user)
        {
            // Lấy thông tin cấu hình từ appsetting.json

            var secret = _configuration["JWT:Secret"];
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];
            var accessTokenExpirationMinutes = int.Parse(_configuration["JWT:AccessTokenExpirationMinutes"]);

            // Tạo danh sách các claim cho mã JWT
            var claims = new List<Claim>{
                new Claim("id", user.UserId.ToString()),
                new Claim("Name", user.Fullname),
                new Claim("Email", user.Email),
            };

            // Tạo key từ secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            
            //lấy giờ hiện tại theo GMT +7
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            var expiresAt = localTime.AddMinutes(accessTokenExpirationMinutes);

            // Tạo mã JWT
            var token = new JwtSecurityToken
                (
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512)
                );

            // Trả về mã JWT dưới dạng string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[255];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private void CreatedPasswordHash(string password, out byte[] passwordHash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }
    }
}
