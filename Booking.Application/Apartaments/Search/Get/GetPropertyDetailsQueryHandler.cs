using Booking.Domain.Apartments.DTOs;
using Booking.Domain.Review;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Search.Get
{
    public class GetPropertyDetailsQueryHandler 
        : IRequestHandler<GetPropertyDetailsQuery, PropertyDetailsDto>
    {

        private readonly IPropertyAvailabilityRepository _availabilityRepository;
        private readonly IApplicationContext _context;

        public GetPropertyDetailsQueryHandler(
            IPropertyAvailabilityRepository availabilityRepository,
            IApplicationContext context)
        {
            _availabilityRepository = availabilityRepository;
            _context = context;
        }

        public async Task<PropertyDetailsDto> Handle(
            GetPropertyDetailsQuery request, CancellationToken ct)
        {
            // calli i pare ne databaze  get property + address
            // join loads only needed columns
            // Select projection merr nga db vetem te dhenat qe ne perdorim 
            var property = await _context.Properties
                .Where(p =>
                    p.Id == request.PropertyId &&
                    p.IsActive == true)
                .Join(
                    _context.Addresses,
                    p => p.AddressId,
                    a => a.Id,
                    (p, a) => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Rules,
                        p.PropertyType,
                        p.MaxGuests,
                        p.MinimumStayNights,
                        p.MaximumStayNights,
                        p.PhotoUrl,
                        p.PricePerNight,
                        p.AverageRating,
                        p.ReviewCount,
                        p.AmenitiesRaw,
                        p.CheckInTime,
                        p.CheckOutTime,
                        Country = a.Country,
                        City = a.City,
                        Street = a.Street,
                        PostalCode = a.PostalCode
                    })
                .FirstOrDefaultAsync(ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            
            var storedMonths = await _availabilityRepository
                .GetAllForPropertyAsync(request.PropertyId, ct);

            var seasonPrices = await _availabilityRepository
                .GetAllSeasonPricesAsync(request.PropertyId, ct);

            var discount = await _availabilityRepository
                .GetDiscountAsync(request.PropertyId, ct);

            var reviews = await _context.Reviews
                .Join(
                    _context.Bookings,
                    r => r.BookingId,
                    b => b.Id,
                    (r, b) => new { Review = r, Booking = b })
                .Where(x => x.Booking.PropertyId == request.PropertyId)
                .Join(
                    _context.Users,
                    x => x.Review.GuestId,
                    u => u.Id,
                    (x, u) => new ReviewDto
                    {
                        Id = x.Review.Id,
                        Rating = x.Review.Rating,
                        Comment = x.Review.Comment,
                        CreatedAt = x.Review.CreatedAt,
                        GuestName = u.Name + " " + u.LastName
                    })
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync(ct);


            

            //Build calendar — 12 months from today
            // if no DB row exists for a month — all days are free
            // if a row exists — use its AvailableDays list
            // current month — strips past days
            var availability = new List<MonthAvailabilityDto>();
            var today = DateTime.UtcNow;

            for (int i = 0; i < 12; i++)
            {
                var date = today.AddMonths(i);
                var year = date.Year;
                var month = date.Month;
                var daysInMonth = DateTime.DaysInMonth(year, month);

                var stored = storedMonths.FirstOrDefault(
                    m => m.Year == year && m.Month == month);

                var availableDays = stored is not null
                    ? stored.AvailableDays
                    : Enumerable.Range(1, daysInMonth).ToList();

                if (year == today.Year && month == today.Month)
                    availableDays = availableDays
                        .Where(d => d >= today.Day)
                        .ToList();

                availability.Add(new MonthAvailabilityDto
                {
                    Year = year,
                    Month = month,
                    AvailableDays = availableDays
                });
            }

            //vendosja e cmimit bazuar ne sezon dhe discount
            var pricing = new PricingBreakdownDto
            {
                BasePricePerNight = property.PricePerNight,
                SeasonPrices = seasonPrices.Select(s => new SeasonPriceDetailDto
                {
                    Season = s.Season.ToString(),
                    PricePerNight = s.PricePerNight
                }).ToList(),
                Discount = discount is null ? null : new DiscountDetailDto
                {
                    MinimumNights = discount.MinimumNights,
                    DiscountPerNight = discount.DiscountPerNight,
                    Description =
                        $"Stay {discount.MinimumNights}+ nights " +
                        $"and save ${discount.DiscountPerNight}/night"
                }
            };

            return new PropertyDetailsDto
            {
                Id = property.Id,
                Name = property.Name,
                Description = property.Description,
                Rules = property.Rules,
                PropertyType = property.PropertyType.ToString(),
                MaxGuests = property.MaxGuests,
                MinimumStayNights = property.MinimumStayNights,
                MaximumStayNights = property.MaximumStayNights,
                PhotoUrl = property.PhotoUrl,
                CheckInHour = property.CheckInTime.Hour,
                CheckOutHour = property.CheckOutTime.Hour,
                Country = property.Country,
                City = property.City,
                Street = property.Street,
                PostalCode = property.PostalCode,
                Amenities = ParseAmenities(property.AmenitiesRaw),
                Pricing = pricing,
                Availability = availability,
                AverageRating = property.AverageRating,
                ReviewCount = property.ReviewCount,
                Reviews = reviews
            };
        }

        private static List<string> ParseAmenities(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return new List<string>();

            return raw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => ((Booking.Domain.Enum.Amenity)int.Parse(a)).ToString())
                .ToList();
        }

    }
}
