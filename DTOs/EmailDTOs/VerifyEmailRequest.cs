﻿using System.ComponentModel.DataAnnotations;

namespace DoAn4.DTOs.EmailDTOs
{
    public class VerifyEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;
    }
}
