using Booking.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Booking.Application.Users
{
    public interface IUserRepository : IGenericRepository <User>
    {
        Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct = default);
    }
}
