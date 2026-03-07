using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments
{
    public class CreatePropertyDto
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; set; }
        public PropertyType PropertyType { get; init; }
        public List<Amenity> Amenities { get; init; } = new();

        public int MaxGuests { get; init; }
        public int CheckInHour { get; init; } = 14;
        public int CheckOutHour { get; init; } = 11;
        public string? Rules { get; set; }
        public decimal PricePerNight { get; init; }

        // Address fields — inline
        public string Country { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;


    }
}
