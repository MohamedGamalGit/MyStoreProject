using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class LoginResponse
    {
        public string Token { get; set; }=null!;
        public string Code { get; set; } = null!;
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string RefreshToken { get; set; } // الحقل الجديد


    }
}
