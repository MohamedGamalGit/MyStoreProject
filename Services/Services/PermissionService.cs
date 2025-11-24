using Commen.ViewModels.Permissions;
using Commen.ViewModels;
using Models.Models.Permission;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Services.Services
{
    public class PermissionService: IPermissionService
    {
        private readonly StoreDbContext _ctx;
        private readonly IMapper _mapper;

        public PermissionService(StoreDbContext ctx, IMapper mapper) { _ctx = ctx; _mapper = mapper; }

        public async Task<List<PageDto>> GetPagesAsync()
            => await _ctx.Page.Select(p => new PageDto { Id = p.Id, Key = p.Key, DisplayName = p.DisplayName }).ToListAsync();

        public async Task<List<ActionDto>> GetActionsAsync()
            => await _ctx.ActionEntity.Select(a => new ActionDto { Id = a.Id, Key = a.Key, DisplayName = a.DisplayName }).ToListAsync();

        public async Task<List<PageActionDto>> GetPageActionsAsync()
        {
            return await _ctx.PageAction
                .Include(pa => pa.Page)
                .Include(pa => pa.ActionEntity)
                .Select(pa => new PageActionDto
                {
                    Id = pa.Id,
                    PageKey = pa.Page.Key,
                    ActionKey = pa.ActionEntity.Key
                }).ToListAsync();
        }

        public async Task AssignRolePermissionsAsync(List<RoleAssignDto> dtos)
        {
            foreach (var dto in dtos)
            {
                // 1. حذف الـ PageActions القديمة المرتبطة بهذا الدور
                var existing = _ctx.RolePageAction
                    .Where(rp => rp.RoleId == dto.RoleId)
                    .ToList();

                _ctx.RolePageAction.RemoveRange(existing);

                // 2. إضافة PageActions الجديدة
                foreach (var pageActionId in dto.PageActionIds.Distinct())
                {
                    _ctx.RolePageAction.Add(new RolePageAction
                    {
                        RoleId = dto.RoleId,
                        PageActionId = pageActionId
                    });
                }
            }

            await _ctx.SaveChangesAsync();
        }


        public async Task<RolePermissionsDto> GetRolePermissionsAsync(Guid roleId)
        {
            var role = await _ctx.Roles.FindAsync(roleId);
            if (role == null) throw new Exception("Role not found");

            var perms = await _ctx.RolePageAction
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.PageAction).ThenInclude(pa => pa.Page)
                .Include(rp => rp.PageAction).ThenInclude(pa => pa.ActionEntity)
                .Select(rp => rp.PageAction.Page.Key + "." + rp.PageAction.ActionEntity.Key)
                .ToListAsync();

            return new RolePermissionsDto
            {
                RoleId = roleId,
                RoleName = role.Name,
                Permissions = perms
            };
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            // 1) Get user + his roles
            var user = await _ctx.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new List<string>();

            // Extract all Role IDs
            var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

            // 2) Get permissions related to these roles
            var permissions = await _ctx.RolePageAction
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Include(rp => rp.PageAction)
                    .ThenInclude(pa => pa.Page)
                .Include(rp => rp.PageAction)
                    .ThenInclude(pa => pa.ActionEntity)
                .Select(rp => rp.PageAction.Page.Key + "." + rp.PageAction.ActionEntity.Key)
                .ToListAsync();

            return permissions;
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionKey)
        {
            // 1️⃣ جلب أدوار المستخدم
            var userRoleIds = await _ctx.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!userRoleIds.Any())
                return false;

            // 2️⃣ جلب PageAction المرتبطة بالأدوار فقط
            var rolePageActions = await _ctx.RolePageAction
                .Where(rp => userRoleIds.Contains(rp.RoleId))
                .Select(rp => new
                {
                    PageKey = rp.PageAction!.Page!.Key,
                    ActionKey = rp.PageAction.ActionEntity!.Key
                })
                .AsNoTracking()
                .ToListAsync(); // ✅ هنا await صحيح

            // 3️⃣ تحقق من الصلاحية على ال Client
            bool hasPermission = rolePageActions
                .Any(r => $"{r.PageKey}.{r.ActionKey}" == permissionKey);

            return hasPermission;
        }

        public async Task AddPageAsync(PageDto dto)
        {
            var model = _mapper.Map<Page>(dto);
            _ctx.Page.Add(model);
            await _ctx.SaveChangesAsync();
        }
    }
}
