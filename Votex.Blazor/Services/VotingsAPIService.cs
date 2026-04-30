using System.Net.Http.Headers;
using Votex.Shared.DTO;

namespace Votex.Blazor.Services
{
    public class VotingsAPIService : IVotingsAPIService
    {
        private readonly HttpClient _httpClient;

        public VotingsAPIService(HttpClient httpClient)
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

        public async Task<bool> CreateVotingAsync(string accessToken, CreateVotingRequestDto dto)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsJsonAsync("votings/create", dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<VotingListingRequestDto>?> GetFinishedVotings(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("votings/finished");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<VotingListingRequestDto>>();

            else
                return null;
        }

        public async Task<List<VotingListingRequestDto>?> GetOngoingVotings(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("votings");

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<VotingListingRequestDto>>();

            else
                return null;
        }

        public async Task<VotingResultRequestDto?> GetResultsAsync(string accessToken, int votingId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("votings/results/" + votingId);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<VotingResultRequestDto>();

            else
                return null;
        }

        public async Task<VotingResponseDto?> GetVotingById(string accessToken, int votingId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("votings/" + votingId);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<VotingResponseDto>();

            else
                return null;
        }

        public async Task<bool> VoteForVotingAsync(string accessToken, VoteRequestDto dto)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsJsonAsync("votings/vote", dto);

            return response.IsSuccessStatusCode;
        }
    }
}
