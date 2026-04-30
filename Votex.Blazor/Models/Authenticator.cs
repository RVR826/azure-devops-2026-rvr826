using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Votex.Blazor.Services;
namespace Votex.Blazor.Models
{
    public class UserState : AuthenticationState
    {
        public UserState(ClaimsPrincipal principal) : base(principal) { }

        public bool IsAuthenticated 
        {
            get => User.Identity!.IsAuthenticated;
        }

        public string? Email
        {
            get => User.Claims?.SingleOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        }        
        
        public string? AccessToken
        {
            get => User.Claims?.SingleOrDefault(x => x.Type == "AccessToken")?.Value;
        }        
        
        public string? RefreshToken
        {
            get => User.Claims?.SingleOrDefault(x => x.Type == "RefreshToken")?.Value;
        }
    }
    
    public class Authenticator : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        private ClaimsPrincipal _anonymous;
        private ClaimsPrincipal _user;

        public Authenticator(ILocalStorageService localStorage) : base()
        {
            _localStorage = localStorage;

            _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            _user = null!;
        }

        public async Task LoginAsync(string email, string accessToken, string refreshToken)
        {
            await _localStorage.SetItemAsync("jwtAccessToken", accessToken);
            await _localStorage.SetItemAsync("jwtRefreshToken", refreshToken);
            await _localStorage.SetItemAsync("email", email);

            await Task.FromResult(() =>
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim("Email", email),
                    new Claim("AccessToken", accessToken),
                    new Claim("RefreshToken", refreshToken)
                }, 
                "CustomAuth");

                _user = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
            });
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("jwtAccessToken");
            await _localStorage.RemoveItemAsync("jwtRefreshToken");
            await _localStorage.RemoveItemAsync("email");

            await Task.FromResult(() =>
            {
                _user = _anonymous;
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
            });
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // first auth state request
            if (_user is null)
            {
                var email = await _localStorage.GetItemAsync("email");
                var accessToken = await _localStorage.GetItemAsync("jwtAccessToken");
                var refreshToken = await _localStorage.GetItemAsync("jwtRefreshToken");
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    _user = _anonymous;
                }
                else
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, email),
                        new Claim("AccessToken", accessToken),
                        new Claim("RefreshToken", refreshToken)
                    },
                    "CustomAuth");

                    _user = new ClaimsPrincipal(identity);
                }
            }

            AuthenticationState state = new UserState(_user);

            return state;
        }
    }

}

