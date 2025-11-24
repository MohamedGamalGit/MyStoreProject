using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Permission
{
    public class RolePageAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public Guid PageActionId { get; set; }
        public PageAction PageAction { get; set; } = null!;
    }
}
