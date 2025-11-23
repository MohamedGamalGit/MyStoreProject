using Infrastructure.Data;
using Models.Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ActivityLogService: IActivityLogService
    {
        private readonly StoreDbContext _context;

        public ActivityLogService(StoreDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string userName, string action,
                                   string entityName, string entityId, string description,string path,string method,string ip,int status,int executionTime,List<string> roles )
        {
            var log = new ActivityLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                Path = path,
                Method = method,
                IpAddress = ip,
                StatusCode = status,
                Timestamp = DateTime.Now,
                Roles= roles

            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
