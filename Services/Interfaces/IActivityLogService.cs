using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IActivityLogService
    {
        Task LogAsync(string userId, string userName, string action,
                                   string entityName, string entityId, string description, string path, string method, string ip, int status, int executionTime, List<string> roles);
    }
}
