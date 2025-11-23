using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Commen.Helpers
{
    public class JwtHelper
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _expireMinutes;

        public JwtHelper(string key, string issuer, string audience, double expireMinutes)
        {
            _key = key;
            _issuer = issuer;
            _audience = audience;
            _expireMinutes = expireMinutes;
        }

        /// <summary>
        /// Generate JWT token for a user with multiple roles
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="username">Username</param>
        /// <param name="roles">List of roles</param>
        /// <returns>JWT token string</returns>
        public string GenerateToken(Guid userId, string username, List<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", userId.ToString()),
                new Claim("UserName", username)
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim("Roles", role));
            }

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
