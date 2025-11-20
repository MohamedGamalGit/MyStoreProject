using Commen.Helpers;
using Commen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using static Commen.Helpers.Helper;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;

        public UsersController(IUserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var token = await _userService.Login(dto);
            if (token == null) return Unauthorized();

            return Ok(token);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateVM dto)
        {
            var user = await _userService.Register(dto);
            return Ok(user); // ممكن ترجع UserViewModel بدون PasswordHash
        }

        [HttpPost("refresh-token")]
        public async Task<LoginResponse> RefreshToken([FromBody] TokenRequest request)
        {
            var user = (await _userService.FindAsync(u => u.RefreshToken == request.RefreshToken)).FirstOrDefault();
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var newJwtToken = _jwtHelper.GenerateToken(user.Id, user.Username, userRoles);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateUserAsync(user);

            return new LoginResponse
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }

        public class TokenRequest
        {
            public string RefreshToken { get; set; }
        }

    }
}
