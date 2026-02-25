using System;
using System.Collections.Generic;
using System.Text;
using Booking.Domain.Users;

namespace Booking.Application
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
