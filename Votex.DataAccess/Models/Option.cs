using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votex.DataAccess.Models
{
    public class Option
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Voting")]
        public int VotingId { get; set; }

        public string Text { get; set; } = null!;

        public virtual Voting Voting { get; set; } = null!;
    }
}