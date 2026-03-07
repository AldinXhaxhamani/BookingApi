using Booking.Application.Roles;
using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly BookingDbContext _context;

        public RoleRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name, ct);
        }
    }
}
