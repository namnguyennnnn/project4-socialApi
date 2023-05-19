
using DoAn4.DTOs.PostDTO;
using DoAn4.DTOs.UserDTO;
using DoAn4.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getFrofile-user"), Authorize]
        public async Task<IActionResult> GetProfileUser()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.GetFrofileUserAsync(token);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }
        [HttpPut("updateProfile-user"), Authorize]
        public async Task<IActionResult> updateProfile([FromForm] UpdateProFileUserDto? updateProFileUserDto = null)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.UpdateProfileUserAsync(token, updateProFileUserDto);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }
        [HttpPut("updateAvatar-user"), Authorize]
        public async Task<IActionResult> UpDateAvatar( IFormFile imageFile)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.UpdateAvatarUserAsync(token, imageFile);
                if(user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }

        [HttpPut("updateCoverPhoto-user"), Authorize]
        public async Task<IActionResult> UpDateCoverPhoto(IFormFile imageFile)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.UpdateCoverPhotoUserAsync(token, imageFile);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }
    }
}
