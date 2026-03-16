using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class PricingBreakdownDto
    {

        public decimal BasePricePerNight { get; set; }
        public List<SeasonPriceDetailDto> SeasonPrices { get; set; } = new();
        public DiscountDetailDto? Discount { get; set; }

    }
}
