using Booking.Application.Apartaments.Filter_Properties;
using Booking.Domain;
using Booking.Domain.Apartments;
using Booking.Domain.Apartments.DTOs;
using Booking.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Booking.Application.Apartaments.Search
{
    public class SearchPropertiesQueryHandler 
        : IRequestHandler<SearchPropertiesQuery,
            PagedResultDto<PropertySearchResultDto>>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyAvailabilityRepository _availabilityRepository;

        public SearchPropertiesQueryHandler(
            IPropertyRepository propertyRepository,
            IPropertyAvailabilityRepository availabilityRepository)
        {
            _propertyRepository = propertyRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<PagedResultDto<PropertySearchResultDto>> Handle(
    SearchPropertiesQuery request, CancellationToken ct)
        {
            //validate
            if (string.IsNullOrWhiteSpace(request.Country))
                throw new InvalidOperationException("Country is required.");

            if (request.Page < 1)
                throw new InvalidOperationException("Page must be at least 1.");

            if (request.PageSize < 1 || request.PageSize > 50)
                throw new InvalidOperationException(
                    "Page size must be between 1 and 50.");

            var hasDateRange = request.CheckIn.HasValue &&
                               request.CheckOut.HasValue;

            if (hasDateRange && request.CheckOut!.Value <= request.CheckIn!.Value)
                throw new InvalidOperationException(
                    "Check-out must be after check-in.");

            if (hasDateRange && request.CheckIn!.Value.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException(
                    "Check-in date cannot be in the past.");

            // te pakten 1  nate qendrim 
            if (hasDateRange && (request.CheckOut!.Value.Date - request.CheckIn!.Value.Date).Days < 1)
                throw new InvalidOperationException(
                    "Stay must be at least 1 night.");


            //  Build filter
            var filter = new PropertySearchFilter
            {
                Country = request.Country,
                City = request.City,
                Guests = request.Guests,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                PropertyType = request.PropertyType,
                MinRating = request.MinRating,
                SortBy=request.SortBy,
                Amenities = string.IsNullOrWhiteSpace(request.Amenities)
                    ? null
                    : request.Amenities
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Where(a => int.TryParse(a, out _))
                        .Select(int.Parse)
                        .ToList()
            };

            //No date range — paginate in DB 
            if (!hasDateRange)
            {
                var (propertyItems, dbTotalCount) = await _propertyRepository
                    .SearchAsync(filter, request.Page, request.PageSize, ct);

                if (!propertyItems.Any())
                    return BuildEmptyResult(request);

                var dbTotalPages = (int)Math.Ceiling(
                    dbTotalCount / (double)request.PageSize);

                return new PagedResultDto<PropertySearchResultDto>
                {
                    Items = propertyItems.Select(p => new PropertySearchResultDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Country = p.Country,
                        City = p.City,
                        Street = p.Street,
                        PropertyType = p.PropertyType,
                        PricePerNight = p.PricePerNight,
                        MaxGuests = p.MaxGuests,
                        MinimumStayNights = p.MinimumStayNights,
                        MaximumStayNights = p.MaximumStayNights,
                        AverageRating = p.AverageRating,
                        ReviewCount = p.ReviewCount,
                        BookingCount = p.BookingCount,
                        PhotoUrl = p.PhotoUrl,
                        Amenities = ParseAmenities(p.AmenitiesRaw)
                    }).ToList(),
                    TotalCount = dbTotalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = dbTotalPages,
                    HasNextPage = request.Page < dbTotalPages,
                    HasPreviousPage = request.Page > 1
                };
            }

            // Date range — fetch all, check availability, paginate in memory
            var properties = await _propertyRepository.SearchAsync(filter, ct);

            if (!properties.Any())
                return BuildEmptyResult(request);

            var ids = properties.Select(p => p.Id).ToList();

            var allMonths = await _availabilityRepository
                .GetForPropertiesAsync(ids, ct);

            var availabilityMap = allMonths
                .GroupBy(a => a.PropertyId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var checkIn = request.CheckIn!.Value.Date;
            var checkOut = request.CheckOut!.Value.Date;

            properties = properties
                .Where(p => IsAvailableForRange(
                    p.Id, checkIn, checkOut, availabilityMap))
                .ToList();

            if (!properties.Any())
                return BuildEmptyResult(request);

            // sort in memory
            properties = request.SortBy switch
            {
                SortBy.Price => properties.OrderBy(p => p.PricePerNight).ToList(),
                SortBy.PriceDesc => properties.OrderByDescending(p => p.PricePerNight).ToList(),
                SortBy.Popularity => properties.OrderByDescending(p => p.BookingCount).ToList(),
                _ => properties.OrderByDescending(p => p.AverageRating).ToList()
            };

            // paginate in memory
            var totalCount = properties.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var paged = properties
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResultDto<PropertySearchResultDto>
            {
                Items = paged.Select(p => new PropertySearchResultDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Country = p.Country,
                    City = p.City,
                    Street = p.Street,
                    PropertyType = p.PropertyType,
                    PricePerNight = p.PricePerNight,
                    MaxGuests = p.MaxGuests,
                    MinimumStayNights = p.MinimumStayNights,
                    MaximumStayNights = p.MaximumStayNights,
                    AverageRating = p.AverageRating,
                    ReviewCount = p.ReviewCount,
                    BookingCount = p.BookingCount,
                    PhotoUrl = p.PhotoUrl,
                    Amenities = ParseAmenities(p.AmenitiesRaw)
                }).ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };
        }


        // funksion ndihmes qe kontrollon ne memorje per available dates 
        private static bool IsAvailableForRange(
            Guid propertyId,
            DateTime checkIn,
            DateTime checkOut,
            Dictionary<Guid, List<PropertyMonthlyAvailability>> map)
        {
            // property not in map = no rows = all dates free
            if (!map.TryGetValue(propertyId, out var months))
                return true;

            var current = checkIn;

            while (current < checkOut) // checkOut excluded — checkout day stays open
            {
                var row = months.FirstOrDefault(m =>
                    m.Year == current.Year &&
                    m.Month == current.Month);

                // no row for this month = all days in month free
                if (row is not null && !row.IsDayAvailable(current.Day))
                    return false;

                current = current.AddDays(1);
            }

            return true;
        }

        // "1,3,5" → ["WiFi", "Parking", "SwimmingPool"]
        private static List<string> ParseAmenities(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return new List<string>();

            return raw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => ((Booking.Domain.Enum.Amenity)int.Parse(a)).ToString())
                .ToList();
        }

        private static PagedResultDto<PropertySearchResultDto> BuildEmptyResult(
            SearchPropertiesQuery request) =>
            new()
            {
                Items = new(),
                TotalCount = 0,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = 0,
                HasNextPage = false,
                HasPreviousPage = false
            };

    }
}
