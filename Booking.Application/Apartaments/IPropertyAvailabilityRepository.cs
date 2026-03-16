using Booking.Domain.Apartments;
using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments
{
    public interface IPropertyAvailabilityRepository
    {

        Task<PropertyMonthlyAvailability?> GetMonthAsync(
        Guid propertyId, int year, int month,
        CancellationToken ct = default);

        
        Task<List<PropertyMonthlyAvailability>> GetAllForPropertyAsync(
            Guid propertyId, CancellationToken ct = default);

       


        Task<bool> AreDatesAvailableAsync(
            Guid propertyId, DateTime startDate, DateTime endDate,
            CancellationToken ct = default);

        

        Task RemoveBookedDatesAsync(
            Guid propertyId, DateTime startDate, DateTime endDate,
            CancellationToken ct = default);

        
        Task BlockDatesAsync(
            Guid propertyId, List<DateTime> dates,
            CancellationToken ct = default);

        
        Task RestoreDatesAsync(
            Guid propertyId, List<DateTime> dates,
            CancellationToken ct = default);

        Task AddAsync(
            PropertyMonthlyAvailability availability,
            CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);

        //season price
        Task<PropertySeasonPrice?> GetSeasonPriceAsync(
            Guid propertyId, Season season,
            CancellationToken ct = default);

        Task<List<PropertySeasonPrice>> GetAllSeasonPricesAsync(
            Guid propertyId, CancellationToken ct = default);

        Task AddSeasonPriceAsync(PropertySeasonPrice seasonPrice,
            CancellationToken ct = default);

        // Discount
        Task<PropertyDiscount?> GetDiscountAsync(
            Guid propertyId, CancellationToken ct = default);

        Task AddDiscountAsync(PropertyDiscount discount,
            CancellationToken ct = default);



        Task<List<PropertyMonthlyAvailability>> GetForPropertiesAsync(
            List<Guid> propertyIds,
            CancellationToken ct = default);

    }


}
