using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Permission
{
    public class Page
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = null!; // e.g. "Users"
        public string DisplayName { get; set; } = null!; // e.g. "Users Management"
        public ICollection<PageAction> PageActions { get; set; } = new List<PageAction>();
    }
}
