using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }
}
