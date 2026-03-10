using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class DiscountDto
    {
        public int MinimumNights { get; set; }
        public decimal DiscountPerNight { get; set; }

    }
}
