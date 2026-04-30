using Votex.Shared.DTO;

namespace Votex.Blazor.Services
{
    public interface IUserAPIService
    {
        Task<List<string>?> GetUserEmailsAsync(string accessToken);
        Task<LoginResponseDto?> LoginToAPIAsync(LoginRegisterRequestDto dto);
        Task<RegisterResponseDto> RegisterToAPIAsync(LoginRegisterRequestDto dto);
    }
}