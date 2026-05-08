using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Votex.Shared.DTO;
using Votex.DataAccess.Models;
using Votex.DataAccess.Services;

namespace Votex.API.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRegisterRequestDto loginRequestDto)
        {
            var loginResponseDto = new LoginResponseDto();

            try
            {
                var (authToken, refreshToken, userId) = await _userService.LoginAsync(loginRequestDto.Email, loginRequestDto.Password);
                loginResponseDto.UserId = userId;
                loginResponseDto.AccessToken = authToken;
                loginResponseDto.RefreshToken = refreshToken;
            }
            catch(AccessViolationException)
            {
                return BadRequest();
            }

            return Ok(loginResponseDto);
        }        
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] LoginRegisterRequestDto registerRequestDto)
        {
            var registerResponseDto = new RegisterResponseDto();
            var user = new User
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                EmailConfirmed = true
            };

            try
            {
                await _userService.AddUserAsync(user, registerRequestDto.Password);
            }
            catch(InvalidDataException ex)
            {
                registerResponseDto.Errors = ex.Message.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                return BadRequest(registerResponseDto);
            }

            return Ok(registerResponseDto);
        }

        [HttpGet]
        [Route("users")]
        //[Authorize]
        public async Task<IActionResult> Users()
        {
            var users = (await _userService.GetUsersAsync()).Select(x => x.Email).ToList();

            return Ok(users);
        }
    }
}
