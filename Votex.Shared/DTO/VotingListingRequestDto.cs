namespace Votex.Shared.DTO
{
    public class VotingListingRequestDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public bool AlreadyVoted { get; set; }
    }
}
