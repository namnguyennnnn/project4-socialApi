
using Microsoft.AspNetCore.Mvc;
using DoAn4.Models;
using DoAn4.Services.EmailService;
using DoAn4.Interfaces;
using DoAn4.Helper;
using DoAn4.DTOs.UserDTOs;
using DoAn4.DTOs.EmailDTOs;

namespace DoAn4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public AccountController(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        //{ 

        //}
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegesterRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                PasswordSalt = PasswordHelper.GenerateSalt(),
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                Fullname = request.Fullname,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                CreateAt = DateTime.UtcNow,
                IsEmailVerified = false
            };

            await _userRepository.CreateUserAsync(newUser);

            var token = await GenerateEmailVerificationToken(request.Email);

            
            var emailBody = $"<p>This is your verify code: </p> <h2>{token}</h2>";
            await _emailService.SendEmailAsync(newUser.Email, "Email Verification", emailBody);

            return Ok(/*"Register success , check your mail to verify"*/ newUser);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Email not found");
            }

            if (user.IsEmailVerified)
            {
                return BadRequest("Email already verified");
            }

            if (user.VerifiedToken != model.Token)
            {
                return BadRequest("Invalid token");
            }

            user.IsEmailVerified = true;
            user.VerifiedToken = null;

            await _userRepository.UpdateUserAsync(user);

            return Ok("Email verified successfully");
        }

        private async Task<string> GenerateEmailVerificationToken(string email)
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
