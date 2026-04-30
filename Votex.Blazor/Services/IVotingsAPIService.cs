using Votex.Shared.DTO;

namespace Votex.Blazor.Services
{
    public interface IVotingsAPIService
    {
        Task<bool> CreateVotingAsync(string accessToken, CreateVotingRequestDto dto);
        Task<List<VotingListingRequestDto>?> GetFinishedVotings(string accessToken);
        Task<List<VotingListingRequestDto>?> GetOngoingVotings(string accessToken);
        Task<VotingResultRequestDto?> GetResultsAsync(string accessToken, int votingId);
        Task<VotingResponseDto?> GetVotingById(string accessToken, int votingId);
        Task<bool> VoteForVotingAsync(string accessToken, VoteRequestDto dto);
    }
}