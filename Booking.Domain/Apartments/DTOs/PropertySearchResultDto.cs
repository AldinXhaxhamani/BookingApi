using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments.DTOs
{
    public class PropertySearchResultDto
    {

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int MinimumStayNights { get; set; }
        public int MaximumStayNights { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int BookingCount { get; set; }
        public string? PhotoUrl { get; set; }
        public List<string> Amenities { get; set; } = new();

    }
}
