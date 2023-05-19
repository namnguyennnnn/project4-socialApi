
using DoAn4.DTOs.UserDTO;
using DoAn4.Models;

namespace DoAn4.Services.UserService
{
    public interface IUserService
    {
        Task<InfoUserDTO> GetFrofileUserAsync(string token);

        Task<InfoUserDTO> UpdateProfileUserAsync(string token, UpdateProFileUserDto? updateProFileUserDto = null);

        Task<InfoUserDTO> UpdateAvatarUserAsync(string token, IFormFile imageFile);

        Task<InfoUserDTO> UpdateCoverPhotoUserAsync(string token, IFormFile imageFile);
    }
}
