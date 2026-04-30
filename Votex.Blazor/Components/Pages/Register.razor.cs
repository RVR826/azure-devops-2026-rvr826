using Microsoft.AspNetCore.Components.Web;
using Votex.Shared.DTO;

namespace Votex.Blazor.Components.Pages
{
    public partial class Register
    {
        private LoginRegisterRequestDto RequestDto = new LoginRegisterRequestDto();
        private RegisterResponseDto ResponseDto = new RegisterResponseDto();
        private List<string> ErrorMessages = new List<string>();

        private async Task TryRegisterAsync()
        {
            ErrorMessages.Clear();
            
            ResponseDto = await API.RegisterToAPIAsync(RequestDto);

            if (ResponseDto.Errors.Count == 0)
            {
                NavManager.NavigateTo("/login", true);
            }

            foreach (var error in ResponseDto.Errors)
            {
                ErrorMessages.Add(error);
            }
        }
        
        private async Task OnKeypressTryRegisterAsync(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await TryRegisterAsync();   
            }
        }
    }
}
