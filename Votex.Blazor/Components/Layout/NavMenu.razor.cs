namespace Votex.Blazor.Components.Layout
{
    public partial class NavMenu
    {
        private async Task LogoutAsync()
        {
            await Authenticator.LogoutAsync();
            NavManager.NavigateTo("/", true);
        }
    }
}
