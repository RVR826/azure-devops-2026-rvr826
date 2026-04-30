using Votex.DataAccess.Models;

namespace Votex.DataAccess.Services
{
    public interface IVotingsService
    {
        Task<Voting> GetVotingById(int id);
        Task<List<Voting>> GetVotingsForUser(User user, bool ongoing);
        Task<List<Voting>> GetVotingsForUser(string email, bool ongoing);
        Task VoteForVoting(Voting voting, Vote vote, User user);
        Task AddVoting(Voting voting);
    }
}