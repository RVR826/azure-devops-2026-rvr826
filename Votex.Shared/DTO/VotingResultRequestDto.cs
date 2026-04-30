namespace Votex.Shared.DTO
{
    public class VotingResultRequestDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public List<string> Options { get; set; } = null!;
        public List<int> VoteCountForOptions { get; set; } = null!;
        public int AllVotersCount { get; set; }
    }
}
