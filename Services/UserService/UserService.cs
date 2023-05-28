
using DoAn4.Interfaces;
using DoAn4.Services.PostService;
using Microsoft.EntityFrameworkCore;
using DoAn4.Services.AuthenticationService;
using DoAn4.DTOs.UserDTO;

using System.Globalization;

namespace DoAn4.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostService _postService;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuthenticationService _authenticationService;

        public UserService(IAuthenticationService authenticationService, IWebHostEnvironment env, IUserRepository userRepository, IPostService postService)
        {
            _userRepository = userRepository;
            _postService = postService;
            _environment = env;
            _authenticationService = authenticationService;
        }

        public async Task<InfoUserDTO> GetFrofileUserAsync(string token)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var user = await _userRepository.GetUserByIdAsync(curUser.UserId);
            var newUserDto = new InfoUserDTO
            {                
                Email = user.Email,
                Fullname = user.Fullname,
                Avatar = Path.Combine(_environment.ContentRootPath, user.Avatar),
                CoverPhoto = Path.Combine(_environment.ContentRootPath, user.CoverPhoto),
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Bio = user.Bio,
                CreateAt =user.CreateAt
            };
            return newUserDto;
        }

        public async Task<InfoUserDTO> UpdateAvatarUserAsync(string token, IFormFile imageFile)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var user = await _userRepository.GetUserByIdAsync(curUser.UserId);
            var imgPath = await _postService.UpdateAvatarAsync(token, imageFile);
            if(imgPath == null)
            {
                throw new ArgumentNullException(nameof(imgPath));
            }
            else if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var imgPathWithRoot = Path.Combine(_environment.ContentRootPath, imgPath);
            
            user.Avatar = imgPathWithRoot;
            
            
            try
            {
                await _userRepository.UpdateUserAsync(user);
                var UserDto = new InfoUserDTO
                {
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    CoverPhoto = user.CoverPhoto,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    Bio = user.Bio
                };
                return UserDto; 
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }

        }

        public async Task<InfoUserDTO> UpdateCoverPhotoUserAsync(string token, IFormFile imageFile)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token);
            var user = await _userRepository.GetUserByIdAsync(curUser.UserId);
            var imgPath = await _postService.UpdateAvatarAsync(token, imageFile);
            if (imgPath == null)
            {
                throw new ArgumentNullException(nameof(imgPath));
            }
            else if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var imgPathWithRoot = Path.Combine(_environment.ContentRootPath, imgPath);

            user.CoverPhoto = imgPathWithRoot;

            try
            {
                await _userRepository.UpdateUserAsync(user);
                var UserDto = new InfoUserDTO
                {
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    CoverPhoto = user.CoverPhoto,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    Bio = user.Bio
                };
                return UserDto;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }

        }

        public async Task<InfoUserDTO> UpdateProfileUserAsync(string token, UpdateProFileUserDto? updateProFileUserDto = null)
        {
            var curUser = await _authenticationService.GetIdUserFromAccessToken(token) ?? throw new ArgumentNullException(nameof(token),"Token hết hạn");
            var user = await _userRepository.GetUserByIdAsync(curUser.UserId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Người dùng không tồn tại");
            }
            else if (updateProFileUserDto?.Fullname == null && updateProFileUserDto?.Address == null && updateProFileUserDto?.DateOfBirth == null)
            {
                throw new ArgumentNullException(nameof(updateProFileUserDto), "Không có thông tin cập nhật nào được cung cấp");
            }
            if (updateProFileUserDto.Fullname !=null)
            {
                user.Fullname = updateProFileUserDto.Fullname;
            }
            if (updateProFileUserDto.Address != null)
            {
                user.Address = updateProFileUserDto.Address;
            }
           
            if (updateProFileUserDto.DateOfBirth != null)
            {
                var formatTime = TimeZoneInfo.ConvertTimeFromUtc(updateProFileUserDto?.DateOfBirth ?? DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                user.DateOfBirth = DateTime.ParseExact(formatTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            try
            {
                await _userRepository.UpdateUserAsync(user);
                var UserDto = new InfoUserDTO
                {
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    CoverPhoto = user.CoverPhoto,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    Bio = user.Bio
                };
                return UserDto;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Cập nhật không thành công: " + ex.Message);
            }
        }
    }
}
