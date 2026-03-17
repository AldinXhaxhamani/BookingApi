using System;
using System.Collections.Generic;
using System.Text;
using Booking.Domain.Bookings;

namespace Booking.Application.Bookings
{
    public  interface IBookingRepository
    {

        Task AddAsync(
            BookingEntity booking,
            CancellationToken ct = default);

        Task<BookingEntity?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);

    }
}
