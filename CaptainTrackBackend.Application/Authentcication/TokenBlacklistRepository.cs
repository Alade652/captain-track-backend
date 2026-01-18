using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainTrackBackend.Application.Context;
using Microsoft.EntityFrameworkCore;

namespace CaptainTrackBackend.Application.Authentcication
{
    public class TokenBlacklistRepository: ITokenBlacklistRepository
    {
        private readonly ApplicationDbContext _context;
        public TokenBlacklistRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(string token, DateTime? expiryDate)
        {
            // Check if token already exists to avoid duplicates
            if (!await ExistsAsync(token))
            {
                var blacklistedToken = new TokenBlacklist
                {
                    Token = token,
                    ExpiryDate = expiryDate
                };

                _context.TokenBlacklists.Add(blacklistedToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string token)
        {
            // Check if token exists and is not expired
            return await _context.TokenBlacklists
                .AnyAsync(t => t.Token == token && t.ExpiryDate > DateTime.UtcNow);
        }
    }
}
