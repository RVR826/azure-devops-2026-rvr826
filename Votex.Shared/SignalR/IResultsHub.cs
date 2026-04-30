namespace Votex.Shared.SignalR
{
    public interface IResultsHub
    {
        public Task ResultChanged(int votingId);
    }
}