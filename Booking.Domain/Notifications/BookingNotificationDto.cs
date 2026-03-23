using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Notifications
{
    public class BookingNotificationDto
    {

        public Guid BookingId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
