using Commen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class UserRole : Audit
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
