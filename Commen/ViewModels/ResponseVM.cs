using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commen.ViewModels
{
    public class ResponseVM<T>
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public List<T>? Data { get; set; }
    }
}
