using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class RoleAssignDto
    {
        public Guid RoleId { get; set; }
        public List<Guid> PageActionIds { get; set; } = new();
    }
}
