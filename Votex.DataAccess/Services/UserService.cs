using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Votex.DataAccess.Config;
using Votex.DataAccess.Models;

namespace Votex.DataAccess.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task AddUserAsync(User user, string password)
        {
            var preAddedUser = _userManager.Users.SingleOrDefault(x => x.Email == user.Email && string.IsNullOrEmpty(x.PasswordHash));

            if (preAddedUser is not null)
            {
                preAddedUser.RefreshToken = Guid.NewGuid();
                await _userManager.AddPasswordAsync(preAddedUser, password);
            }
            else
            {
                user.RefreshToken = Guid.NewGuid();

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        sb.Append(error.Description + ",");
                    }

                    throw new InvalidDataException(sb.ToString());
                }
            }
        }

        public async Task<(string authToken, string refreshToken, int userId)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new AccessViolationException("Email or password is invalid");

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, password, false, true);
            if (!result.Succeeded)
                throw new AccessViolationException("Email or password is invalid");

            var accessToken = await GenerateJwtTokenAsync(user);

            return (accessToken, user.RefreshToken.ToString()!, user.Id);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<(string authToken, string refreshToken, int userId)> RedeemRefreshTokenAsync(string refreshToken)
        {
            if (!Guid.TryParse(refreshToken, out var parsedToken))
                throw new AccessViolationException("Invalid refresh token");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == parsedToken);
            if (user == null)
                throw new AccessViolationException("Invalid refresh token");

            var accessToken = await GenerateJwtTokenAsync(user);

            return (accessToken, refreshToken, user.Id);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("id", user.Id.ToString()),
                new("username", user.UserName!)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
