using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Votex.Blazor.Services;

namespace Votex.Blazor.HubServices
{
    public abstract class BaseHubService : IBaseHubService
    {
        protected HubConnection? _hubConnection;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILocalStorageService _localStorageService;

        protected BaseHubService(JsonSerializerOptions jsonOptions,
            ILocalStorageService localStorageService)
        {
            _jsonOptions = jsonOptions;
            _localStorageService = localStorageService;
        }

        protected void InitHub(string hubName)
        {
            var fullUri = new Uri(new Uri("https://votex-api.azurewebsites.net/"), hubName);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(fullUri, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var token = await _localStorageService.GetItemAsync("JwtAccessToken");
                        return token;
                    };
                })
                .AddJsonProtocol(config =>
                {
                    config.PayloadSerializerOptions = _jsonOptions;
                })
                .WithAutomaticReconnect()
                .Build();
        }

        protected async Task ConnectHubAsync()
        {
            if (_hubConnection!.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task DisconnectHubAsync()
        {
            if (_hubConnection!.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}
