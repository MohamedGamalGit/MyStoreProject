using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels.RolesVM
{
    public class RoleAddVM
    {
        public string? Name { get; set; } = null!; // Admin, User, Guest
        public string? RoleName { get; set; }  // Admin, User, Guest
        public Guid? Id { get; set; }
    }
}
