using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string Email { get; set; }
        public List<string>? UserRoles { get; set; }
    }

}
