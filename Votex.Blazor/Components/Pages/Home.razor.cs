using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Votex.Blazor.Models;
using Votex.Shared.DTO;

namespace Votex.Blazor.Components.Pages
{
    public partial class Home
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
        private UserState User = null!;

        private List<VotingListingRequestDto> RequestDtos = new List<VotingListingRequestDto>();
        private List<VotingListingRequestDto> FilteredRequestDtos = new List<VotingListingRequestDto>();
        private string QuestionFilter = null!;
        private DateTime StartFilter = DateTime.Now;
        private DateTime EndFilter = DateTime.Now.AddYears(2);

        protected override async Task OnInitializedAsync()
        {
            // prevent static analizer warning
            if (QuestionFilter is null)
                QuestionFilter = null!;

            AuthenticationState = Authenticator.GetAuthenticationStateAsync();
            User = (UserState)await AuthenticationState;

            if (User.IsAuthenticated && RequestDtos.Count == 0)
            {
                RequestDtos = (await API.GetOngoingVotings(User.AccessToken!))!;

                if (RequestDtos is not null)
                {
                    FilteredRequestDtos = new(RequestDtos);
                }
                else
                {
                    await Authenticator.LogoutAsync();
                    AuthenticationState = Authenticator.GetAuthenticationStateAsync();
                    User = (UserState)await AuthenticationState;
                    StateHasChanged();
                }
            }
        }

        private void GoToVoting(int id)
        {
            NavManager.NavigateTo("/vote/" + id, true);
        }
        
        private void GoToVotingResults(int id)
        {
            NavManager.NavigateTo("/results/" + id, true);
        }

        private void FilterVotings(string? question, DateTime start, DateTime end)
        {
            FilteredRequestDtos = RequestDtos.Where(x => DateTime.Parse(x.EndDate) >= start && DateTime.Parse(x.EndDate) <= end).ToList();         
            
            if (question is not null)
                FilteredRequestDtos = FilteredRequestDtos.Where(x => x.Question.ToLower().Contains(question.ToLower())).ToList();            
        }
    }
}
