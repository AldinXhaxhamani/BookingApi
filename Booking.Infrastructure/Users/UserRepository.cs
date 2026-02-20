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
    }
}
