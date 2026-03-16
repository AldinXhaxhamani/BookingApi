using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class SeasonPriceDetailDto
    {
        public string Season { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
    }
}
