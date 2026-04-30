using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votex.DataAccess.Models
{
    public class Voting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Question { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool AreLiveResultsOn { get; set; }

        public virtual List<Vote> Votes { get; set; } = null!;

        public virtual List<User> Users { get; set; } = null!;

        public virtual List<Option> Options { get; set; } = null!;

        public virtual List<User> AlreadyVoted { get; set; } = null!;
    }
}
