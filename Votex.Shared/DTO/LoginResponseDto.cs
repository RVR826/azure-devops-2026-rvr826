namespace Votex.Shared.DTO
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
