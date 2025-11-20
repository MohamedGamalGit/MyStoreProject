using Commen.ViewModels;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<LoginResponse> Login(LoginRequest dto);
        Task<UserViewModel> Register(UserCreateVM user);
        Task<IEnumerable<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate);
        Task UpdateUserAsync(User user);
    }
}
