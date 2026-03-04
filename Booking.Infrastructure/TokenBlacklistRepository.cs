using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure
{
    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {

        private readonly BookingDbContext _context;

        public TokenBlacklistRepository(BookingDbContext context)
        {
            _context = context;
        }




        public async Task AddAsync(BlacklistedToken token, CancellationToken ct = default)
        {
             await _context.BlacklistedTokens.AddAsync(token, ct);
             await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> IsBlacklistedAsync(string token, CancellationToken ct = default)
        {
           
            return await _context.BlacklistedTokens
            .AnyAsync(b => b.Token == token, ct);
        }
    }
}
