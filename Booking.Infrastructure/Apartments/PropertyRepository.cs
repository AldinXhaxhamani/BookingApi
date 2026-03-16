using Booking.Application.Apartaments;
using Booking.Application.Apartaments.Filter_Properties;
using Booking.Application.Apartaments.Search;
using Booking.Domain.Apartments;
using Booking.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure.Apartments
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        private readonly BookingDbContext _dbContext;

        public PropertyRepository(BookingDbContext dbContext) : base(dbContext)
        {

            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
        {
            var isUnique = await _dbContext.Properties
                .Where(c => c.Name == name).ToListAsync(cancellationToken);
            return isUnique.Count == 0; //meqe me siper na kthehet nje lsite me propertioes me kte emer 

        }


        public async Task<List<PropertySearchProjection>> SearchAsync(
    PropertySearchFilter filter, CancellationToken ct = default)
        {
            //marrim vetem properties qe jane aktive dhe approved 
            var query = _dbContext.Properties
                .Where(p => p.IsActive == true && p.IsApproved == true)
                .Join(
                    _dbContext.Addresses,
                    p => p.AddressId,
                    a => a.Id,
                    (p, a) => new { Property = p, Address = a });

            //vendosim country si mandatory dhe filterat e tjera si optional
            //clienti mund te vendosi vlera vetem ne disa filtra 
            query = query.Where(x =>
                x.Address.Country.ToLower() == filter.Country.ToLower());

            //city
            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x =>
                    x.Address.City.ToLower().Contains(filter.City.ToLower()));

            //max number of guests
            if (filter.Guests.HasValue)
                query = query.Where(x =>
                    x.Property.MaxGuests >= filter.Guests.Value);

            // price range 
            if (filter.MinPrice.HasValue)
                query = query.Where(x =>
                    x.Property.PricePerNight >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x =>
                    x.Property.PricePerNight <= filter.MaxPrice.Value);

            // Property type 
            if (filter.PropertyType.HasValue)
                query = query.Where(x =>
                    x.Property.PropertyType == filter.PropertyType.Value);

            // Rating
            if (filter.MinRating.HasValue)
                query = query.Where(x =>
                    x.Property.AverageRating >= filter.MinRating.Value);

            // Amenities 
            // property duhet te kete te gjitha amenties e kerkuara nga klienti 
            if (filter.Amenities is { Count: > 0 })
            {
                foreach (var amenity in filter.Amenities)
                {
                    var value = amenity.ToString();
                    query = query.Where(x =>
                        x.Property.AmenitiesRaw != null &&
                        x.Property.AmenitiesRaw.Contains(value));
                }
            }

            

            return await query
                .Select(x => new PropertySearchProjection
                {
                    Id = x.Property.Id,
                    Name = x.Property.Name,
                    Description = x.Property.Description,
                    Country = x.Address.Country,
                    City = x.Address.City,
                    Street = x.Address.Street,
                    PropertyType = x.Property.PropertyType.ToString(),
                    PricePerNight = x.Property.PricePerNight,
                    MaxGuests = x.Property.MaxGuests,
                    MinimumStayNights = x.Property.MinimumStayNights,
                    MaximumStayNights = x.Property.MaximumStayNights,
                    AverageRating = x.Property.AverageRating,
                    ReviewCount = x.Property.ReviewCount,
                    BookingCount = x.Property.BookingCount,
                    PhotoUrl = x.Property.PhotoUrl,
                    AmenitiesRaw = x.Property.AmenitiesRaw
                })
                .ToListAsync(ct);

        }

            public async Task<(List<PropertySearchProjection> Items, int TotalCount)> SearchAsync(
                PropertySearchFilter filter,
                int page,
                int pageSize,
                CancellationToken ct = default)
            {
                var query = _dbContext.Properties
                .Where(p => p.IsActive == true && p.IsApproved == true)
                .Join(
                    _dbContext.Addresses,
                    p => p.AddressId,
                    a => a.Id,
                    (p, a) => new { Property = p, Address = a });

                query = query.Where(x =>
                    x.Address.Country.ToLower() == filter.Country.ToLower());

                if (!string.IsNullOrWhiteSpace(filter.City))
                    query = query.Where(x =>
                        x.Address.City.ToLower().Contains(filter.City.ToLower()));

                if (filter.Guests.HasValue)
                    query = query.Where(x =>
                        x.Property.MaxGuests >= filter.Guests.Value);

                if (filter.MinPrice.HasValue)
                    query = query.Where(x =>
                        x.Property.PricePerNight >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    query = query.Where(x =>
                        x.Property.PricePerNight <= filter.MaxPrice.Value);

                if (filter.PropertyType.HasValue)
                    query = query.Where(x =>
                        x.Property.PropertyType == filter.PropertyType.Value);

                if (filter.MinRating.HasValue)
                    query = query.Where(x =>
                        x.Property.AverageRating >= filter.MinRating.Value);

                if (filter.Amenities is { Count: > 0 })
                {
                    foreach (var amenity in filter.Amenities)
                    {
                        var value = ((int)amenity).ToString();
                        query = query.Where(x =>
                            x.Property.AmenitiesRaw != null &&
                            x.Property.AmenitiesRaw.Contains(value));
                    }
                }

                // count total BEFORE pagination — one DB call
                var totalCount = await query.CountAsync(ct);

                var sorted = filter.SortBy switch
                {
                    SortBy.Price => query.OrderBy(x => x.Property.PricePerNight),
                    SortBy.PriceDesc => query.OrderByDescending(x => x.Property.PricePerNight),
                    SortBy.Popularity => query.OrderByDescending(x => x.Property.BookingCount),
                    _ => query.OrderByDescending(x => x.Property.AverageRating)
                };



            // then fetch only the current page — second DB call
            var items = await query
                .OrderByDescending(x=>x.Property.AverageRating)
                .Select(x => new PropertySearchProjection
                {
                    Id = x.Property.Id,
                    Name = x.Property.Name,
                    Description = x.Property.Description,
                    Country = x.Address.Country,
                    City = x.Address.City,
                    Street = x.Address.Street,
                    PropertyType = x.Property.PropertyType.ToString(),
                    PricePerNight = x.Property.PricePerNight,
                    MaxGuests = x.Property.MaxGuests,
                    MinimumStayNights = x.Property.MinimumStayNights,
                    MaximumStayNights = x.Property.MaximumStayNights,
                    AverageRating = x.Property.AverageRating,
                    ReviewCount = x.Property.ReviewCount,
                    BookingCount = x.Property.BookingCount,
                    PhotoUrl = x.Property.PhotoUrl,
                    AmenitiesRaw = x.Property.AmenitiesRaw
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

                return (items, totalCount);
            }



    }
    
}
