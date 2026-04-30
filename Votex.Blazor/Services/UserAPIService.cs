using System.Net.Http.Headers;
using Votex.Shared.DTO;

namespace Votex.Blazor.Services
{
    public class UserAPIService : IUserAPIService
    {
        private readonly HttpClient _httpClient;

        public UserAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>?> GetUserEmailsAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("auth/users");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<string>>();

            else
                return null;
        }

        public async Task<LoginResponseDto?> LoginToAPIAsync(LoginRegisterRequestDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", dto);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            else
                return null;
        }

        public async Task<RegisterResponseDto> RegisterToAPIAsync(LoginRegisterRequestDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", dto);

            return (await response.Content.ReadFromJsonAsync<RegisterResponseDto>())!;
        }
    }
}
