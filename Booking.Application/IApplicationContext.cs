using Booking.Domain.Apartments;
using Booking.Domain.Entities;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application
{
    public interface IApplicationContext
    {
        public DbSet<User> Users { get; }

        public DbSet<Property> Properties { get; }
        public DbSet<OwnerProfile> Owners { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

