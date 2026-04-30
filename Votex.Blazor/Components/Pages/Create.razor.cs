using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Votex.Blazor.Models;
using Votex.Shared.DTO;
using BlazorBootstrap;

namespace Votex.Blazor.Components.Pages
{
    public partial class Create
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
        private UserState User = null!;

        private CreateVotingRequestDto RequestDto = new CreateVotingRequestDto();
        private List<string> UserEmails = new List<string>();
        private List<string> ErrorMessages = new List<string>();
        private string MiscUser = null!;
        private string NewOption = null!;
        private bool ModalInputError = false;
        private DateTime StartDate = DateTime.Now;
        private DateTime EndDate = DateTime.Now;

        private Modal AddMiscUserModal = default!;
        private Modal AddOptionModal = default!;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = Authenticator.GetAuthenticationStateAsync();
            User = (UserState)await AuthenticationState;

            if (!User.IsAuthenticated)
            {
                NavManager.NavigateTo("/login", true);
            }

            RequestDto.UserEmails = new List<string>();
            RequestDto.Options = new List<string>();

            if (UserEmails.Count == 0)
            {
                UserEmails = (await UsersAPI.GetUserEmailsAsync(User.AccessToken!))!;
            }
        }

        private async Task TryCreate()
        {
            ErrorMessages.Clear();

            if (RequestDto.UserEmails.Count < 2)
                ErrorMessages.Add("At least 2 people needed for a valid voting");

            if (RequestDto.Options.Count < 2)
                ErrorMessages.Add("At least 2 options needed for a valid voting");

            if (string.IsNullOrEmpty(RequestDto.Question))
                ErrorMessages.Add("A question is needed for a valid voting");

            if (StartDate < DateTime.Now || EndDate < DateTime.Now)
                ErrorMessages.Add("Invalid start or end date");

            if (StartDate.AddMinutes(15) > EndDate)
                ErrorMessages.Add("The voting duration must be at lest 15 minutes");

            if (User.IsAuthenticated && ErrorMessages.Count == 0)
            {
                RequestDto.StartDate = StartDate.ToString("g");
                RequestDto.EndDate = EndDate.ToString("g");

                var success = await VotingsAPI.CreateVotingAsync(User.AccessToken!, RequestDto);

                if (success)
                {
                    NavManager.NavigateTo("/", true);
                }
            }
        }

        private async Task AddMiscUserAsync()
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(MiscUser) && !RequestDto.UserEmails.Contains(MiscUser))
                {
                    ModalInputError = false;

                    RequestDto.UserEmails.Add(MiscUser);
                    MiscUser = null!;
                }
                else
                {
                    ModalInputError = true;
                }
            });

            if (!ModalInputError)
            {
                await AddMiscUserModal.HideAsync();
            }
        }

        private async Task AddOptionAsync()
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(NewOption) && !RequestDto.Options.Contains(NewOption))
                {
                    ModalInputError = false;
                    
                    RequestDto.Options.Add(NewOption);
                    NewOption = null!;
                }
                else
                {
                    ModalInputError = true;
                }
            });

            if (!ModalInputError)
            {
                await AddOptionModal.HideAsync();
            }
        }
    }
}
