using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ActivityLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public string? Action { get; set; }

        public string? EntityName { get; set; }

        public string? EntityId { get; set; }

        public string? Description { get; set; }

        public string? Path { get; set; }

        public string? Method { get; set; }

        public string? IpAddress { get; set; }

        public int? StatusCode { get; set; }

        public DateTime? Timestamp { get; set; } = DateTime.Now;
        public List<string>? Roles { get; set; } =new List<string>();
    }
}
