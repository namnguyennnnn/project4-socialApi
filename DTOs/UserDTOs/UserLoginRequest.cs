using System.ComponentModel.DataAnnotations;

namespace DoAn4.DTOs.UserDTOs
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    }
}
