
using Microsoft.AspNetCore.Mvc;
using DoAn4.Services.EmailService;
using DoAn4.Interfaces;
using DoAn4.Services.AuthenticationService;
using DoAn4.DTOs;
using DoAn4.DTOs.UserDTO;
using Microsoft.AspNetCore.Authorization;

namespace DoAn4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IUserRepository userRepository, IEmailService emailService, IAuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserLoginDTO request)
        {
             var result = await _authenticationService.LoginAsync(request.Email, request.Password);

             if (!result.Success)
             {
                 return BadRequest(new { errors = result.Errors });
             }

             return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
            
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDTO request)
        {
            try
            {
                var result = await _authenticationService.RegisterAsync(request);

                if (!result)
                {
                    return BadRequest();
                }

                var token = await GenerateOTP(request.Email);

                var emailBody = $"<p>This is your verify code: </p> <h2>{token}</h2>";
                await _emailService.SendEmailAsync(request.Email, "Email Verification", emailBody);

                return Ok("Đăng ký thành công, kiểm tra email để lấy mã xác thực");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromForm] VerifyEmailRequest model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("Email không tồn tại");
                }

                if (user.IsEmailVerified)
                {
                    return BadRequest("Email đã xác thực từ trước đó rồi ");
                }

                if (user.VerifiedToken != model.Token)
                {
                    return BadRequest("OTP không đúng");
                }

                user.IsEmailVerified = true;
                user.VerifiedToken = null;

                await _userRepository.UpdateUserAsync(user);

                return Ok("Xác thực email thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error verifying email: {ex.Message}");
            }
        }


        [HttpPost("logout") ,Authorize]
        public async Task<IActionResult> Logout(string refreshToken)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _authenticationService.LogoutAsync(refreshToken,accessToken);
                if(result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Xảy ra lỗi khi đăng xuất: {ex.Message}");
            }
        }

        [HttpPost("renewToken"), Authorize]
        public async Task<IActionResult> RenewToken(string refreshToken)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authenticationService.RefreshAccesstokenTokenAsync(refreshToken, accessToken);

            if (!result.Success)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });

        }

        [NonAction]
        private async Task<string> GenerateOTP(string email)
        {
            Random random = new Random();
            string token = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            var user = await _userRepository.GetUserByEmailAsync(email);
            user.VerifiedToken = token;
            await _userRepository.UpdateUserAsync(user);
            return token;
        }
    }
}
