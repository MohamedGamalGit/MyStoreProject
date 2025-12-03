using Infrastructure.Data;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TokenCleanupService : ITokenCleanupService
    {
        private readonly StoreDbContext _context;

        public TokenCleanupService(StoreDbContext context)
        {
            _context = context;
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expired = _context.Users
                .Where(t => t.RefreshTokenExpiryTime < DateTime.UtcNow);

            foreach (var item in expired)
            {
                item.RefreshTokenExpiryTime = null;
                item.RefreshToken = null;
            }
            await _context.SaveChangesAsync();

            Console.WriteLine("Expired tokens cleaned!");
        }
    }
}
