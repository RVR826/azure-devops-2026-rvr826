using Microsoft.EntityFrameworkCore;
using Votex.DataAccess.Models;

namespace Votex.DataAccess.Services
{
    public class VotingsService : IVotingsService
    {
        private readonly VotexDbContext _context;

        public VotingsService(VotexDbContext context)
        {
            _context = context;
        }

        public async Task<Voting> GetVotingById(int id)
        {
            return await _context.Votings.SingleAsync(x => x.Id == id);
        }

        public async Task<List<Voting>> GetVotingsForUser(User user, bool ongoing)
        {
            return await GetVotingsForUser(user.Email!, ongoing);
        }   

        public async Task<List<Voting>> GetVotingsForUser(string email, bool ongoing)
        {
            var votings = ongoing ?
                _context.Votings.Where(x => x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate && x.AlreadyVoted.Count < x.Users.Count && x.Users.Any(x => x.Email == email)) :
                _context.Votings.Where(x => (x.EndDate <= DateTime.Now || x.AlreadyVoted.Count == x.Users.Count) && x.Users.Any(x => x.Email == email));

            return await votings
                .OrderBy(x => x.EndDate)
                .ToListAsync();
        }

        public async Task VoteForVoting(Voting voting, Vote vote, User user)
        {
            if (!voting.Options.Contains(vote.Option) && !voting.Options.Select(x => x.Id).Contains(vote.OptionId))
            {
                throw new ArgumentException("Not a valid vote for specified voting!");
            }

            if (!voting.Users.Contains(user))
            {
                throw new ArgumentException("This user cannot vote for this voting!");
            }            
            
            if (voting.AlreadyVoted.Contains(user))
            {
                throw new ArgumentException("This user has already voted!");
            }

            voting.Votes.Add(vote);
            voting.AlreadyVoted.Add(user);

            await _context.SaveChangesAsync();
        }

        public async Task AddVoting(Voting voting)
        {
            if (voting.Options.Count < 2)
            {
                throw new ArgumentException("A voting must have at least 2 options!");
            }
            
            if (voting.Users.Count < 2)
            {
                throw new ArgumentException("A voting must have at least 2 added users!");
            }

            if (voting.StartDate < DateTime.Now || voting.EndDate < DateTime.Now)
            {
                throw new ArgumentException("Invalid start or end date: both start and end has to be in the future!");
            }

            if (voting.StartDate.AddMinutes(15) > voting.EndDate)
            {
                throw new ArgumentException("Invalid start or end date: there must be at least 15 minutes between the start and end!");
            }

            _context.Votings.Add(voting);

            await _context.SaveChangesAsync();
        }
    }
}
