using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Entities
{
    public class BlacklistedToken
    {

        public Guid Id { get; set; }
        public string Token { get;  set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime InvalidatedAt { get;  set; }

        private BlacklistedToken() { }

        public static BlacklistedToken Create(string token, DateTime expiresAt)
        {
            return new BlacklistedToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                ExpiresAt = expiresAt,
                InvalidatedAt = DateTime.UtcNow
            };

        }
    }
}
