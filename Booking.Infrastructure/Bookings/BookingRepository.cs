using Booking.Application.Bookings;
using Microsoft.EntityFrameworkCore;
using Booking.Domain.Bookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure.Bookings
{
    public class BookingRepository : IBookingRepository
    {

        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
            => _context = context;

        public async Task AddAsync(
            BookingEntity booking,
            CancellationToken ct = default)
        {
            await _context.Bookings.AddAsync(booking, ct);
        }

        public async Task<BookingEntity?> GetByIdAsync(
            Guid id, CancellationToken ct = default)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);

    }
}
