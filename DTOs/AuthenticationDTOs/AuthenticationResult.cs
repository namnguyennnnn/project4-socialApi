namespace DoAn4.DTOs.AuthenticationDTOs
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string[]? Errors { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string Avatar { get; set; }
        public string CoverPhoto { get; set; }
        public string FullName { get; set; }
        public DateTime? DayOfBirth { get; set; }       
    }
}
