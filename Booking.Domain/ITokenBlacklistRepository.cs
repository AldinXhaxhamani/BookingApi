using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure
{
    public interface ITokenBlacklistRepository
    {
        Task<bool> IsBlacklistedAsync(string token, CancellationToken ct = default);
        Task AddAsync(BlacklistedToken token, CancellationToken ct = default);

    }
}
