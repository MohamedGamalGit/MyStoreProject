using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels.RolesVM
{
    public class UserRoleAddVM
    {
        public Guid UserId { get; set; }    // ID المستخدم
        public Guid RoleId { get; set; }     // ID الدور
    }
}
