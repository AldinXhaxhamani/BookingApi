using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Dtos
{
    public class AuthResponse
    {

        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public IEnumerable<string> Roles { get; set; } = [];
        public DateTime ExpiresAt { get; set; }


    }
}
