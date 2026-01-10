using AutoMapper;
using Commen.ViewModels.RolesVM;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;

        public RolesController(StoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Endpoint لإضافة Role جديد (Admin فقط)
        [HttpPost("add")]
        public async Task<IActionResult> AddRole([FromBody] RoleAddVM role)
        {
            if (string.IsNullOrWhiteSpace(role.Name))
                return BadRequest("Role name is required.");

            // تحقق إذا الدور موجود بالفعل
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == role.Name.ToLower());

            if (existingRole != null)
                return BadRequest("Role already exists.");
            var model = _mapper.Map<Role>(role);
            model.Id = Guid.NewGuid();
            _context.Roles.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role added successfully", role });
        }

        [HttpPost("assign-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleAddVM dto)
        {
            try
            {
                // التحقق من وجود المستخدم
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                if (user == null)
                    return NotFound("User not found");

                // التحقق من وجود الدور
                var role = await _context.Roles.FindAsync(dto.RoleId);
                if (role == null)
                    return NotFound("Role not found");

                // التحقق إذا الدور موجود مسبقاً للمستخدم
                bool alreadyAssigned = user.UserRoles.Any(ur => ur.RoleId == dto.RoleId);
                if (alreadyAssigned)
                    return BadRequest("User already has this role");

                // إضافة الدور
                _context.UserRoles.Add(new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId
                });

                await _context.SaveChangesAsync();


                return Ok(new { message = $"Role '{role.Name}' assigned to user '{user.Username}' successfully" });
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        [HttpGet("getAll")]
        //[Authorize(Roles = "SuperAdmin")] // اختياري: فقط SuperAdmin يمكنه جلب الأدوار
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles
                .Select(r => new
                {
                    r.Id,
                    RoleName= r.Name,
                    r.Name,
                    RoleId=r.Id
                })
                .ToListAsync();

            return Ok(roles);
        }
    }

}
