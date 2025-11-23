using Commen.ViewModels;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public LogsController(StoreDbContext context)
        {
            _context = context;
        }
        //[HttpGet("activity-logs")]
        //public async Task<IActionResult> GetLogs()
        //{
        //    return Ok(await _context.ActivityLogs
        //        .OrderByDescending(l => l.Timestamp)
        //        .ToListAsync());
        //}
        [HttpPost("activity-logs")]
        public async Task<ActionResult<Pagination<ActivityLog>>> GetCategoriesWithagination([FromBody] PaginationVM model)
        {
            var logs = await _context.ActivityLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            var pagination = new Pagination<ActivityLog>
            {
                TotalRecords = logs.Count(),
                TotalPages = (int)Math.Ceiling((double)logs.Count() / model.Size),
                Data = logs
                        .Skip((model.Page - 1) * model.Size)
                        .Take(model.Size)
                        .ToList()
            };

            return Ok(pagination);
        }
    }
}
