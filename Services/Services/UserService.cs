using AutoMapper;
using Commen.Helpers;
using Commen.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositories.IGenericRepository;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Commen.Helpers.Helper;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtHelper _jwtHelper;
        private readonly StoreDbContext _context;

        public UserService(IGenericRepository<User> userRepository, IMapper mapper, JwtHelper jwtHelper, StoreDbContext context)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
            _context = context;
        }
        //public async Task<LoginResponse> Login(LoginRequest dto)
        //{
        //    var user = (await _userRepository.FindAsync(u => u.Username == dto.Username)).FirstOrDefault();
        //    if (user == null) return null;

        //    bool isValid = PasswordHasher.VerifyPassword(dto.Password, user.Salt, user.PasswordHash);
        //    if (!isValid) return null;

        //    // Generate JWT token
        //    var token= _jwtHelper.GenerateToken(user.Id, user.Username);
        //    LoginResponse loginResponse = new LoginResponse();
        //    loginResponse.Token = token;
        //    return loginResponse;

        //}
        public async Task<LoginResponse> Login(LoginRequest dto)
        {
            var user = await _context.Users
                                    .Include(u => u.UserRoles)
                                        .ThenInclude(ur => ur.Role)
                                    .FirstOrDefaultAsync(u => u.Username == dto.Username);

            var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            if (user == null) return null;

            bool isValid = PasswordHasher.VerifyPassword(dto.Password, user.Salt, user.PasswordHash);
            if (!isValid) return null;

            // Generate JWT token
            var token = _jwtHelper.GenerateToken(user.Id, user.Username, userRoles);

            // Generate Refresh Token
            var refreshToken = GenerateRefreshToken();

            // تخزين الـ Refresh Token في قاعدة البيانات
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(1); // مثال: 7 أيام
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            // إعداد Response
            LoginResponse loginResponse = new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return loginResponse;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var users = await _context.Users.Include(x=>x.UserRoles).ThenInclude(x=>x.Role).ToListAsync();

            return _mapper.Map<IEnumerable<UserViewModel>>(users.ToList());
        }

        public async Task<UserViewModel> Register(UserCreateVM dto)
        {
            // Generate hash + salt
            string hash = PasswordHasher.HashPassword(dto.Password, out string salt);

            var user = new User
            {
                Email = dto.Email,
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = hash,
                Salt = salt
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            if (dto.UserRoles != null && dto.UserRoles.Any())
            {
                foreach (var r in dto.UserRoles)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = r.Id??new Guid()
                    };

                    await _context.UserRoles.AddAsync(userRole);
                }

                await _context.SaveChangesAsync();
            }
            return _mapper.Map<UserViewModel>(user);
        }
        //public async Task<User> FindAsync(Guid id)
        //{
        //    var user = await _userRepository.GetByIdAsync(id);
        //    return user;
        //}
        public async Task<IEnumerable<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
        {
            var users = await _userRepository.FindAsync(predicate);
            return users;
        }
        public async Task UpdateUserAsync(User user)
        {
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

    }
}
