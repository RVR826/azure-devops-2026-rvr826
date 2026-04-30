using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using Votex.Shared.DTO;
using Votex.Blazor.Services;

namespace Votex.Blazor.HubServices
{
    public class ResultsHubService : BaseHubService
    {
        public event Action<VotingResultRequestDto>? OnVoteReceived;

        public ResultsHubService(JsonSerializerOptions jsonOptions, ILocalStorageService localStorageService)
            : base(jsonOptions, localStorageService)
        {
        }

        public async Task StartHubAsync()
        {
            InitHub("ResultsHub");

            _hubConnection!.On<VotingResultRequestDto>("ResultChanged", dto => OnVoteReceived?.Invoke(dto));

            await ConnectHubAsync();
        }
    }
}
