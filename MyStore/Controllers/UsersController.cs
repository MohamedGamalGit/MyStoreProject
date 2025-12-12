using Commen.Extensions;
using Commen.Helpers;
using Commen.ViewModels;
using Commen.ViewModels.Employees;
using Commen.ViewModels.Permissions;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Services.Interfaces;
using Services.Services;
using static Commen.Helpers.Helper;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;
        private readonly IActivityLogService _activityLogService;
        private readonly StoreDbContext _context;

        public UsersController(IUserService userService, JwtHelper jwtHelper, IActivityLogService activityLogService, StoreDbContext context)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
            _activityLogService = activityLogService;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var token = await _userService.Login(dto);
            if (token == null) return Unauthorized();
            await _activityLogService.LogAsync(
                        userId: null,
                        userName: dto.Username,
                        roles: null,
                            action: "Login",
                        entityName: "Auth",
                        entityId: null,
                        description: "User logged in",
                        path: HttpContext.Request.Path,
                        method: HttpContext.Request.Method,
                        ip: HttpContext.Connection.RemoteIpAddress?.ToString(),
                        status: 200,
                        executionTime: 0
                    );
            return Ok(token);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateVM dto)
        {
            var user = await _userService.Register(dto);
            BackgroundJob.Enqueue<IEmailService>(emailService =>
             emailService.SendWelcomeEmailAsync(dto.Email, dto.Email));

            return Ok(user); // ممكن ترجع UserViewModel بدون PasswordHash
        }
        [HttpGet("getAllUsers")]
        public async Task<ActionResult<ResponseVM<UserCreateVM>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(new ResponseVM<UserCreateVM>()
            {
                Code = 200,
                Message = "Success",
                Data = users.Select(u => new UserCreateVM
                {
                    Username = u.UserName,
                    Email = u.Email
                    // لا ترجع PasswordHash
                }).ToList()
            });
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
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            await _userService.UpdateUserAsync(user);

            return new LoginResponse
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken
            };
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetUserMenu()
        {
            var userId = User.GetUserId(); // from JWT

            // 1) جميع PageActions للـ user من الـ roles + الـ direct assigned
            var userRolePermissions = await _context.RolePageAction
                .Where(rp => rp.Role.UserRoles.Any(u => u.UserId == userId))
            .Select(rp => rp.PageActionId)
            .ToListAsync();

            var userDirectPermissions = await _context.UserPageAction
                .Where(up => up.UserId == userId)
                .Select(up => up.PageActionId)
                .ToListAsync();

            var allUserPermissions = userRolePermissions
                .Union(userDirectPermissions)
                .ToList();

            // 2) الصفحات التي يمتلك المستخدم أحد Actions الخاصة بها
            var pages = await _context.Page
                .Where(p => p.PageActions.Any(pa => allUserPermissions.Contains(pa.Id)))
                .Select(p => new MenuPageDto
                {
                    Key = p.Key,
                    DisplayName = p.DisplayName,
                    Route = "/" + p.Key,
                    Icon = "menu" // أو خزّن icon في الـ DB لو تريد
                })
                .ToListAsync();

            return Ok(pages);
        }


    }
    public class TokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
