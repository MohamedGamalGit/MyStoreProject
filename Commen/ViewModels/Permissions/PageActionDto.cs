using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class PageActionDto
    {
        public Guid Id { get; set; }
        public string PageKey { get; set; } = "";
        public string ActionKey { get; set; } = "";
        public string Key => $"{PageKey}.{ActionKey}";
    }
}
