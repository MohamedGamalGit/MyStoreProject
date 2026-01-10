using Commen.ViewModels;
using Models.Models.Permission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class User: Audit
    {
        [Key]

        public Guid Id { get; set; }
        public string Username { get; set; }=null!;
        public string NameAR { get; set; }=null!;
        public string NameEN { get; set; }=null!;
        public string Email { get; set; }=string.Empty;
        public string PasswordHash { get; set; }= null!;
        public string Salt { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserPageAction> UserPageActions { get; set; } = new List<UserPageAction>();


    }
}
