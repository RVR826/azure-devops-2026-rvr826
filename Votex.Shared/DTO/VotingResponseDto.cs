namespace Votex.Shared.DTO
{
    public class VotingResponseDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = null!;
        public int[] OptionIds { get; set; } = null!;
        public string[] OptionValues { get; set; } = null!;
    }
}
