using Booking.Domain.Apartments.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Availability
{
    public class GetPropertyAvailabilityQueryHandler
        : IRequestHandler<GetPropertyAvailabilityQuery, PropertyAvailabilityDto>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyAvailabilityRepository _availabilityRepository;

        public GetPropertyAvailabilityQueryHandler(
            IPropertyRepository propertyRepository,
            IPropertyAvailabilityRepository availabilityRepository)
        {
            _propertyRepository = propertyRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<PropertyAvailabilityDto> Handle(
            GetPropertyAvailabilityQuery request, CancellationToken ct)
        {
            
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            //marrim muajt e ruajtur ne db per kete property
            var storedMonths = await _availabilityRepository
                .GetAllForPropertyAsync(request.PropertyId, ct);

            //iniciojme nje liste qe do mbaje objektet DTO(datat per cdo muaj) 
            var availability = new List<MonthAvailabilityDto>();
            

            var today = DateTime.UtcNow;


            //marrim 12 muajt e ardhshem duke filluar nga sot
            for (int i = 0; i < 12; i++)
            {   

                var date = today.AddMonths(i);
                var year = date.Year;
                var month = date.Month;
                var daysInMonth = DateTime.DaysInMonth(year, month);

                // marrim te dhenat qe kemi ne db per nje muaj
                var stored = storedMonths
                    .FirstOrDefault(m => m.Year == year && m.Month == month);

                var availableDays = stored is not null
                    ? stored.AvailableDays //nese ekzistojne available dates ne db, perdor keto te dhena 
                    : Enumerable.Range(1, daysInMonth).ToList();//nese nuk ka te dhena ne db ruaj ne liste te gjithe muajin free

                // per muajin ku jemi heqim ditet qe kan kaluar 
                if (year == today.Year && month == today.Month)
                    availableDays = availableDays
                        .Where(d => d >= today.Day)
                        .ToList();


                //shtojme muaji ne liste 
                availability.Add(new MonthAvailabilityDto
                {
                    Year = year,
                    Month = month,
                    AvailableDays = availableDays
                });

            }

            //marrim cmimin per sezonin dhe discountin
            var seasons = await _availabilityRepository
                .GetAllSeasonPricesAsync(request.PropertyId, ct);

            var discount = await _availabilityRepository
                .GetDiscountAsync(request.PropertyId, ct);

            return new PropertyAvailabilityDto
            {
                PropertyId = property.Id,
                MinimumStayNights = property.MinimumStayNights,
                MaximumStayNights = property.MaximumStayNights,
                BasePricePerNight = property.PricePerNight,
                Availability = availability,
                SeasonPrices = seasons.Select(s => new SeasonPriceDetailDto
                {
                    Season = s.Season.ToString(),
                    PricePerNight = s.PricePerNight
                }).ToList(),
                Discount = discount is null ? null : new DiscountDetailDto
                {
                    MinimumNights = discount.MinimumNights,
                    DiscountPerNight = discount.DiscountPerNight
                }
            };
        }

    }
}
