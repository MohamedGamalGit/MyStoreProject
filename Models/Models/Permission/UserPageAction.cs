using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Permission
{
    public class UserPageAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid PageActionId { get; set; }
        public PageAction PageAction { get; set; } = null!;
    }
}
