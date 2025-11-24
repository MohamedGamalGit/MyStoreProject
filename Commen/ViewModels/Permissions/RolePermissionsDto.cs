using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels.Permissions
{
    public class RolePermissionsDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public List<string> Permissions { get; set; } = new();
    }
}
