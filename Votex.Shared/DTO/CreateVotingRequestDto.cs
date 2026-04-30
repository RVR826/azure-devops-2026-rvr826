namespace Votex.Shared.DTO
{
    public class CreateVotingRequestDto
    {
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = null!;
        public List<string> UserEmails { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public bool AreLiveResultsEnabled { get; set; }
    }
}
