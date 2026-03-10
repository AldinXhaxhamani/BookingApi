using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class MonthAvailabilityDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<int> AvailableDays { get; set; } = new();
    }
}
