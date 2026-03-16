using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Enum
{
    public enum SortBy
    {
        Price = 1,          // cheapest first
        PriceDesc = 2,      // most expensive first
        Rating = 3,         // highest rated first
        Popularity = 4      // most booked first
    }
}
