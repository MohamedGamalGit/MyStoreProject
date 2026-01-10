using Commen.ViewModels.RolesVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class UserCreateVM
    {
		public Guid Id { get; set; }
		public string Username { get; set; } = null!;
        public string? Password { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string NameAR { get; set; } = null!;
        public string NameEN { get; set; } = null!;
        public List<RoleAddVM>? UserRoles { get; set; } = new List<RoleAddVM>();
    }
}
