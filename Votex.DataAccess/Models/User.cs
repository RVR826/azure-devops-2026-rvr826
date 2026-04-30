using Microsoft.AspNetCore.Identity;

namespace Votex.DataAccess.Models
{
    public class User : IdentityUser<int>
    {
        public Guid? RefreshToken { get; set; }
        
        public virtual List<Voting> Votings { get; set; } = null!;

        public virtual List<Voting> AlreadyVotedFor { get; set; } = null!;
    }
}