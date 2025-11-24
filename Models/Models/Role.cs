using Commen.ViewModels;
using Models.Models.Permission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Role: Audit
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!; // Admin, User, Guest
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePageAction> RolePageActions { get; set; } = new List<RolePageAction>();

    }
}
