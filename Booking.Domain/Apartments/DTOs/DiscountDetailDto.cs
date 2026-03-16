using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class DiscountDetailDto
    {
        public int MinimumNights { get; set; }
        public decimal DiscountPerNight { get; set; }

        public string Description { get; set; }

    }
}
