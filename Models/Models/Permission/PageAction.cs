using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Permission
{
    public class PageAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PageId { get; set; }
        public Page Page { get; set; } = null!;
        public Guid ActionEntityId { get; set; }
        public ActionEntity ActionEntity { get; set; } = null!;
        public string Key => $"{Page.Key}.{ActionEntity.Key}"; // e.g. "Users.View"
    }
}
