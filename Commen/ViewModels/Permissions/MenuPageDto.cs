using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels.Permissions
{
    public class MenuPageDto
    {
        public string Key { get; set; } = null!;
        public string DisplayName { get; set; }
        public string Route { get; set; }
        public string Icon { get; set; } 
    }
}
