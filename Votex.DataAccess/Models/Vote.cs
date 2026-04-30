using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votex.DataAccess.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Voting")]
        public int VotingId { get; set; }

        [ForeignKey("Option")]
        public int OptionId { get; set; }

        public virtual Voting Voting { get; set; } = null!;

        public virtual Option Option { get; set; } = null!;
    }
}