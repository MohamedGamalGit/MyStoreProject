using AutoMapper;
using Azure.Core;
using Commen.Extensions;
using Commen.Helpers;
using Commen.ViewModels;
using Commen.ViewModels.Employees;
using Commen.ViewModels.Permissions;
using Commen.ViewModels.RolesVM;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Services.Interfaces;
using Services.Services;
using System.Security.Claims;
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
		private readonly IMapper _mapper;

		public UsersController(IUserService userService, JwtHelper jwtHelper, IActivityLogService activityLogService, StoreDbContext context, IMapper mapper)
		{
			_userService = userService;
			_jwtHelper = jwtHelper;
			_activityLogService = activityLogService;
			_context = context;
			_mapper = mapper;
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
            string lang = HttpContext.Request.Headers["Accept-Language"]
               .FirstOrDefault() ?? "ar";

            var users = await _userService.GetAllAsync();
			return Ok(new ResponseVM<UserCreateVM>()
			{
				Code = 200,
				Message = "Success",
				Data = users.Select(u => new UserCreateVM
				{
					Id = u.Id,
					Username = u.UserName,
					Email = u.Email,
					Name= lang == "ar" ? u.NameAR : u.NameEN,	
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
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ResponseVM<UserCreateVM>>> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user =await _userService.GetByUserName(userId);

            return Ok(new ResponseVM<UserCreateVM>
            {
                Code = 200,
                Message = "Success",
                Data = new List<UserCreateVM> { _mapper.Map<UserCreateVM>(user)  }
            });
        }

        [HttpGet("GetUserById/{id}")]
		public async Task<ActionResult<ResponseVM<UserCreateVM>>> GetById(Guid id)
		{

			var user = (await _userService.FindAsync(u => u.Id == id)).FirstOrDefault();
			if (user == null)
			{
				return NotFound(new ResponseVM<UserCreateVM>
				{
					Code = 404,
					Message = "User not found",
					Data = null
				});
			}
			var userRoles = await _context.UserRoles.Where(x => x.UserId == user.Id).Include(x => x.Role).Select(x => x.Role).ToListAsync();
			var userDto = new UserCreateVM
			{
				Id=user.Id,
				Username = user.Username,
				Email = user.Email,
				UserRoles = _mapper.Map<List<RoleAddVM>>(userRoles)
			};
			return Ok(new ResponseVM<UserCreateVM>
			{
				Code = 200,
				Message = "Success",
				Data = new List<UserCreateVM> { userDto }
			});

		}

		[HttpPost("updateUser")]
		public async Task<ActionResult<ResponseVM<UserCreateVM>>> UpdateUser(UserCreateVM user)
		{
			try
			{
				var user1 = (await _userService.FindAsync(u => u.Id == user.Id)).FirstOrDefault();

				user1.Username = user.Username;
				user1.Email = user.Email;
				user1.Id = user.Id;
				await _context.SaveChangesAsync();
				if (user.UserRoles != null && user.UserRoles.Any())
				{
					var oldRoles = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
					_context.UserRoles.RemoveRange(oldRoles);
					await _context.SaveChangesAsync();
					foreach (var r in user.UserRoles)
					{
						var userRole = new UserRole
						{
							UserId = user.Id,
							RoleId = r.RoleId ?? new Guid()
						};

						await _context.UserRoles.AddAsync(userRole);
					}

					await _context.SaveChangesAsync();
					return Ok(new ResponseVM<UserCreateVM>()
					{
						Code = 200,
						Message = "Success",
						Data = [user]

					});
				}
				return Ok(new ResponseVM<UserCreateVM>()
				{
					Code = 200,
					Message = "Success",
					Data = [user]

				});
			}
			catch (Exception ex)
			{
				return BadRequest(new ResponseVM<UserCreateVM>()
				{
					Code = 400,
					Message = "Error",
					Data = [user]

				});
				throw;
			}


		}
		[HttpDelete("deleteUser/{id}")]
		public async Task<ActionResult<ResponseVM<UserCreateVM>>> DeleteUser(Guid id)
		{
			try
			{
				var user = (await _userService.FindAsync(u => u.Id == id)).FirstOrDefault();
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
				return Ok(new ResponseVM<UserCreateVM>()
				{
					Code = 200,
					Message = "User deleted successfully",
					Data = null
				});
			}
			catch (Exception ex)
			{

				return BadRequest(new ResponseVM<UserCreateVM>()
				{
					Code = 200,
					Message = "User Not Found",
					Data = null
				});
			}
			
		}
	}
	public class TokenRequest
	{
		public string RefreshToken { get; set; }
	}
}
