using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class PropertyAvailabilityDto
    {

        public Guid PropertyId { get; set; }
        public int MinimumStayNights { get; set; }
        public int MaximumStayNights { get; set; }
        public decimal BasePricePerNight { get; set; }
        public List<MonthAvailabilityDto> Availability { get; set; } = new();
        public List<SeasonPriceDto> SeasonPrices { get; set; } = new();
        public DiscountDto? Discount { get; set; }

    }
}
