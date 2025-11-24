using Commen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Permission;
using Services.Interfaces;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _svc;

        public PermissionsController(IPermissionService svc) { _svc = svc; }

        [HttpGet("pages")]
        public async Task<IActionResult> GetPages() => Ok(await _svc.GetPagesAsync());

        [HttpPost("addPage")]
        public async Task<IActionResult> AddPage(PageDto pageDto)
        {

            await _svc.AddPageAsync(pageDto);
            return Ok();
        }

        [HttpGet("actions")]
        public async Task<IActionResult> GetActions() => Ok(await _svc.GetActionsAsync());

        [HttpGet("page-actions")]
        public async Task<IActionResult> GetPageActions() => Ok(await _svc.GetPageActionsAsync());

        [HttpPost("assign")]
        public async Task<IActionResult> AssignToRole([FromBody] List<RoleAssignDto> dto)
        {
            await _svc.AssignRolePermissionsAsync(dto);
            return Ok();
        }

        [HttpGet("role/{roleId}/permissions")]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
            => Ok(await _svc.GetRolePermissionsAsync(roleId));
    }
}
