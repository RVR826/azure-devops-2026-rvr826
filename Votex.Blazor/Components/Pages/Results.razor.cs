using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Votex.Blazor.Models;
using Votex.Shared.DTO;

namespace Votex.Blazor.Components.Pages
{
    public partial class Results
    {
        [Parameter]
        public string VotingId { get; set; } = null!;
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
        private UserState User = null!;

        Grid<(string option, int count)> resultsGrid = default!;
        
        private VotingResultRequestDto RequestDto = new VotingResultRequestDto();
        private List<(string option, int count)> VotingResultData = new List<(string option, int count)>();
        private int AllVotesCount = 0;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = Authenticator.GetAuthenticationStateAsync();
            User = (UserState)await AuthenticationState;

            if (User.IsAuthenticated)
            {
                HubService.OnVoteReceived += dto =>
                {
                    if (dto.Id == RequestDto.Id)
                    {
                        RequestDto = dto;

                        AllVotesCount = RequestDto.VoteCountForOptions.Sum();

                        for (int i = 0; i < RequestDto.Options.Count; i++)
                        {
                            VotingResultData[i] = (RequestDto.Options[i], RequestDto.VoteCountForOptions[i]);
                        }
                        
                        InvokeAsync(() =>
                        {
                            resultsGrid.RefreshDataAsync();
                            StateHasChanged();
                        });
                    }
                };

                await HubService.StartHubAsync();

                RequestDto = (await API.GetResultsAsync(User.AccessToken!, Convert.ToInt32(VotingId)))!;

                if (RequestDto is not null)
                {
                    AllVotesCount = RequestDto.VoteCountForOptions.Sum();

                    for (int i = 0; i < RequestDto.Options.Count; i++)
                    {
                        VotingResultData.Add((RequestDto.Options[i], RequestDto.VoteCountForOptions[i]));
                    }
                }
                else
                {
                    NavManager.NavigateTo("/error", true);
                }
            }
            else
            {
                NavManager.NavigateTo("/login", true);
            }

            await base.OnInitializedAsync();
        }
    }
}
