using Microsoft.AspNetCore.Components.Web;
using Votex.Blazor.Models;
using Votex.Shared.DTO;

namespace Votex.Blazor.Components.Pages
{
    public partial class Login
    {
        private LoginRegisterRequestDto RequestDto = new LoginRegisterRequestDto();
        private LoginResponseDto ResponseDto = new LoginResponseDto();
        private bool LoginError = false;

        private async Task TryLoginAsync()
        {
            ResponseDto = (await API.LoginToAPIAsync(RequestDto))!;

            if (ResponseDto is null && !LoginError)
            {
                LoginError = true;
                return;
            }

            await Authenticator.LoginAsync(RequestDto.Email, ResponseDto!.AccessToken, ResponseDto.RefreshToken);

            //Navigates to Home
            NavManager.NavigateTo("/", true);
        }

        private async Task OnKeypressTryLoginAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await TryLoginAsync();
            }
        }
    }
}
