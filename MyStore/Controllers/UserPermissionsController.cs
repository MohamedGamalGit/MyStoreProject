using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models.Permission;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionsController : ControllerBase
    {
        private readonly StoreDbContext _ctx;
        public UserPermissionsController(StoreDbContext ctx) { _ctx = ctx; }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPermissions(Guid userId)
        {
            var user = await _ctx.Users
                .Include(u => u.UserPageActions)
                .ThenInclude(upa => upa.PageAction)
                    .ThenInclude(pa => pa.Page)
                .Include(u => u.UserPageActions)
                .ThenInclude(upa => upa.PageAction)
                    .ThenInclude(pa => pa.ActionEntity)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var structured = user.UserPageActions
                                .Select(upa => new
                                {
                                    PageKey = upa.PageAction.Page.Key,
                                    ActionKey = upa.PageAction.ActionEntity.Key
                                })
                                .GroupBy(x => x.PageKey)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(a => a.ActionKey).ToList()
                                );


            return Ok(new
            {
                userId = user.Id,
                permissions = user.UserPageActions.Select(upa => upa.PageAction.Key).ToList(),
                structured
            });
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AssignPermissions(Guid userId, [FromBody] List<Guid> pageActionIds)
        {
            var user = await _ctx.Users
                .Include(u => u.UserPageActions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // حذف الصلاحيات القديمة
            _ctx.UserPageAction.RemoveRange(user.UserPageActions);

            // إضافة الصلاحيات الجديدة
            foreach (var paId in pageActionIds)
            {
                _ctx.UserPageAction.Add(new UserPageAction
                {
                    UserId = userId,
                    PageActionId = paId
                });
            }

            await _ctx.SaveChangesAsync();
            return Ok();
        }
    }
}
