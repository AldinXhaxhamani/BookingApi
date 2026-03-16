using Booking.Domain.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class PropertyDetailsDto
    {

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Rules { get; set; }
        public string PropertyType { get; set; } = string.Empty;
        public int MaxGuests { get; set; }
        public int MinimumStayNights { get; set; }
        public int MaximumStayNights { get; set; }
        public string? PhotoUrl { get; set; }
        public int CheckInHour { get; set; }
        public int CheckOutHour { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // cmimi i plote =base price + seasonal + discount
        public PricingBreakdownDto Pricing { get; set; } = new();

        // 12 muaj availability per property
        public List<MonthAvailabilityDto> Availability { get; set; } = new();

        // latest 10 reviews
        public List<ReviewDto> Reviews { get; set; } = new();

    }
}
