using Booking.Application.Apartaments;
using Booking.Domain.Apartments;
using Booking.Domain.Enum;
using Microsoft.EntityFrameworkCore;


namespace Booking.Infrastructure.Apartments
{
    public  class PropertyAvailabilityRepository : IPropertyAvailabilityRepository
    {

        private readonly BookingDbContext _context;

        public PropertyAvailabilityRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<PropertyMonthlyAvailability?> GetMonthAsync(
            Guid propertyId, int year, int month,
            CancellationToken ct = default)
        {
            return await _context.PropertyAvailabilities
                .FirstOrDefaultAsync(a =>
                    a.PropertyId == propertyId &&
                    a.Year == year &&
                    a.Month == month, ct);
        }

        public async Task<List<PropertyMonthlyAvailability>> GetAllForPropertyAsync(
            Guid propertyId, CancellationToken ct = default)
        {
            return await _context.PropertyAvailabilities
                .Where(a => a.PropertyId == propertyId)
                .OrderBy(a => a.Year)
                .ThenBy(a => a.Month)
                .ToListAsync(ct);
        }

        public async Task<bool> AreDatesAvailableAsync(
            Guid propertyId, DateTime startDate, DateTime endDate,
            CancellationToken ct = default)
        {
            
            var current = startDate.Date;

            while (current < endDate.Date)
            {
                var monthRow = await GetMonthAsync(
                    propertyId, current.Year, current.Month, ct);

                // nese muaji nuk ekziston, por ne duam ta kontrollojme, ai krijohet ne moment  
                // me te gjithe datat e lira 
                if (monthRow is null)
                {
                    current = current.AddDays(1);//shtohen ditet 1 nga 1 
                    continue;
                }
                    
                if (!monthRow.IsDayAvailable(current.Day))
                    return false;

                current = current.AddDays(1); // kontrollon edhe nese ka dite ne nje muaj pasardhes i cili ekziston dhe ka dite te zena
            }

            return true;
        }

        public async Task RemoveBookedDatesAsync(
            Guid propertyId, DateTime startDate, DateTime endDate,
            CancellationToken ct = default)
        {
            // group dates by year+month — one update per affected month row
            var datesByMonth = GetDatesBetween(startDate, endDate)
                .GroupBy(d => new { d.Year, d.Month });

            foreach (var group in datesByMonth)
            {
                var monthRow = await GetOrCreateMonthAsync(
                    propertyId, group.Key.Year, group.Key.Month, ct);

                var days = group.Select(d => d.Day).ToList();
                monthRow.RemoveDays(days);
            }
        }

        public async Task BlockDatesAsync(
            Guid propertyId, List<DateTime> dates,
            CancellationToken ct = default)
        {
            var datesByMonth = dates
                .Select(d => d.Date)
                .GroupBy(d => new { d.Year, d.Month });

            foreach (var group in datesByMonth)
            {
                var monthRow = await GetOrCreateMonthAsync(
                    propertyId, group.Key.Year, group.Key.Month, ct);

                var days = group.Select(d => d.Day).ToList();
                monthRow.RemoveDays(days);
            }
        }

        public async Task RestoreDatesAsync(
            Guid propertyId, List<DateTime> dates,
            CancellationToken ct = default)
        {
            var datesByMonth = dates
                .Select(d => d.Date)
                .GroupBy(d => new { d.Year, d.Month });

            foreach (var group in datesByMonth)
            {
                var monthRow = await GetMonthAsync(
                    propertyId, group.Key.Year, group.Key.Month, ct);

                if (monthRow is null) continue; // nothing to restore, bejme continue per te pare nese kemi 2 muaj

                var days = group.Select(d => d.Day).ToList();
                monthRow.RestoreDays(days);
            }
        }

        public async Task AddAsync(
            PropertyMonthlyAvailability availability,
            CancellationToken ct = default)
        {
            await _context.PropertyAvailabilities.AddAsync(availability, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);


        //Season prices
        public async Task<PropertySeasonPrice?> GetSeasonPriceAsync(
            Guid propertyId, Season season, CancellationToken ct = default)
        {
            return await _context.PropertySeasonPrices
                .FirstOrDefaultAsync(s =>
                    s.PropertyId == propertyId &&
                    s.Season == season, ct);
        }

        public async Task<List<PropertySeasonPrice>> GetAllSeasonPricesAsync(
            Guid propertyId, CancellationToken ct = default)
        {
            return await _context.PropertySeasonPrices
                .Where(s => s.PropertyId == propertyId)
                .OrderBy(s => s.Season)
                .ToListAsync(ct);
        }

        public async Task AddSeasonPriceAsync(
            PropertySeasonPrice seasonPrice, CancellationToken ct = default)
        {
            await _context.PropertySeasonPrices.AddAsync(seasonPrice, ct);
        }

        // Discount
        public async Task<PropertyDiscount?> GetDiscountAsync(
            Guid propertyId, CancellationToken ct = default)
        {
            return await _context.PropertyDiscounts
                .FirstOrDefaultAsync(d => d.PropertyId == propertyId, ct);
        }

        public async Task AddDiscountAsync(
            PropertyDiscount discount, CancellationToken ct = default)
        {
            await _context.PropertyDiscounts.AddAsync(discount, ct);
        }


        public async Task<List<PropertyMonthlyAvailability>> GetForPropertiesAsync(
            List<Guid> propertyIds, CancellationToken ct = default)

        {
            return await _context.PropertyAvailabilities
                .Where(a => propertyIds.Contains(a.PropertyId))
                .ToListAsync(ct);
        }






        //funksione ndihmes
        private async Task<PropertyMonthlyAvailability> GetOrCreateMonthAsync(
            Guid propertyId, int year, int month, CancellationToken ct)
        {
            var existing = await GetMonthAsync(propertyId, year, month, ct);

            if (existing is not null)
                return existing;

            // first time this month is touched — create pre-populated row
            var newRow = PropertyMonthlyAvailability.CreateForMonth(
                propertyId, year, month);

            await _context.PropertyAvailabilities.AddAsync(newRow, ct);
            return newRow;
        }





        // returns all dates between startDate (inclusive) and endDate (exclusive)
        private static IEnumerable<DateTime> GetDatesBetween(
            DateTime startDate, DateTime endDate)
        {
            var current = startDate.Date;
            while (current < endDate.Date)
            {
                yield return current;//perdoret ne vend te returnit, me eficente, nuk ruan gjithe listen ne memorje 
                current = current.AddDays(1);
            }
        }
    }
}
