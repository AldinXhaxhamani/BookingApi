using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Roles
{
    public interface IRoleRepository
    {

        Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);

    }
}
