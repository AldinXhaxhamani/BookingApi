using Booking.Application.Users;
using Booking.Domain.Users;
using Booking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure.Users
{
    public class UserRepository: GenericRepository <User>, IUserRepository
    {
        private readonly BookingDbContext _dbContext;

        public UserRepository (BookingDbContext dbContext) : base (dbContext)
        {
           _dbContext = dbContext;
        }


        public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct = default)
        {
            return await _dbContext.Users
                .Include(u => u.UserRoles)       // JOIN Users me UserRoles
                    .ThenInclude(ur => ur.Role)  // JOIN UserRoles me Roles
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }


    }
}
