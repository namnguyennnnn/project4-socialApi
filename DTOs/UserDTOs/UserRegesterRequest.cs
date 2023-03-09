using System.ComponentModel.DataAnnotations;

namespace DoAn4.DTOs.UserDTOs
{
    public class UserRegesterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Fullname { get; set; } = null!;

        [Required]
        public int Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = null!;
    }
}
