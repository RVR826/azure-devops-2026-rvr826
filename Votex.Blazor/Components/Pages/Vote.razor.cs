using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using Votex.Blazor.Models;
using Votex.Shared.DTO;

namespace Votex.Blazor.Components.Pages
{
    public partial class Vote
    {
        [Parameter]
        public string VotingId { get; set; } = null!;
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

        private VoteRequestDto RequestDto = new VoteRequestDto();
        private VotingResponseDto ResponseDto = new VotingResponseDto();
        private UserState User = null!;
        private List<(int id, string option)> Options = new List<(int id, string option)>();
        private bool VoteError = false;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = Authenticator.GetAuthenticationStateAsync();
            User = (UserState)await AuthenticationState;

            if (User.IsAuthenticated)
            {
                ResponseDto = (await API.GetVotingById(User.AccessToken!, Convert.ToInt32(VotingId)))!;

                if (ResponseDto is not null)
                {
                    for (int i = 0; i < ResponseDto.OptionIds.Length; i++)
                    {
                        Options.Add((ResponseDto.OptionIds[i], ResponseDto.OptionValues[i]));
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

        private async Task TryVote()
        {
            VoteError = false;
            
            if (RequestDto.OptionId == 0 && !VoteError)
            {
                VoteError = true;
            }

            RequestDto.VotingId = Convert.ToInt32(VotingId);

            var success = await API.VoteForVotingAsync(User.AccessToken!, RequestDto);

            if (success)
            {
                NavManager.NavigateTo("/", true);
            }
        }
    }
}
