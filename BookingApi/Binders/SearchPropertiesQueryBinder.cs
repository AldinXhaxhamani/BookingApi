using Booking.Application.Apartaments.Search;
using Booking.Domain.Enum;

namespace BookingApi.Binders
{
    public class SearchPropertiesQueryBinder
    {

        public static SearchPropertiesQuery BindFromRequest(HttpContext context)
        {
            var q = context.Request.Query;

            return new SearchPropertiesQuery
            {
                Country = q["country"].FirstOrDefault() ?? string.Empty,
                City = q["city"].FirstOrDefault(),
                Amenities = q["amenities"].FirstOrDefault(),
                Guests = int.TryParse(q["guests"], out var g)
                                ? g : null,
                MinPrice = decimal.TryParse(q["minPrice"], out var minP)
                                ? minP : null,
                MaxPrice = decimal.TryParse(q["maxPrice"], out var maxP)
                                ? maxP : null,
                MinRating = double.TryParse(q["minRating"], out var r)
                                ? r : null,
                Page = int.TryParse(q["page"], out var p)
                                ? p : 1,
                PageSize = int.TryParse(q["pageSize"], out var ps)
                                ? ps : 10,
                CheckIn = DateTime.TryParse(q["checkIn"], out var ci)
                                ? ci : null,
                CheckOut = DateTime.TryParse(q["checkOut"], out var co)
                                ? co : null,
                PropertyType = int.TryParse(q["propertyType"], out var pt)
                                ? (PropertyType)pt : null,
                SortBy = int.TryParse(q["sortBy"], out var sb)
                                ? (SortBy)sb : null
            };
        }

    }
}
