using Commen.ViewModels.Permissions;
using Commen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Models.Permission;
using static Services.Services.PermissionService;

namespace Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<PageDto>> GetPagesAsync();
        Task<List<ActionDto>> GetActionsAsync();
        Task<List<PageActionDto>> GetPageActionsAsync();
        Task AssignRolePermissionsAsync(List<RoleAssignDto> dto);
        Task<RolePermissionsDto> GetRolePermissionsAsync(Guid roleId);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
        Task<bool> UserHasPermissionAsync(Guid userId, string permissionKey);
        Task AddPageAsync(PageDto dto);
        Task<List<RoleWithPermissionsDto>> GetUserRolesWithPermissionsAsync(Guid userId);
        Task<List<RoleWithPermissionsDto>> GetRolesWithPermissionsAsync();

    }
}
