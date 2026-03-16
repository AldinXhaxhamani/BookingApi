using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Filter_Properties
{
    public class PropertySearchFilter
    {

        public string Country { get; set; } = string.Empty;  // mandatory
        public string? City { get; set; }
        public int? Guests { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public PropertyType? PropertyType { get; set; }
        public List<int>? Amenities { get; set; }
        public double? MinRating { get; set; }

        public SortBy? SortBy { get; set; }

    }
}
