using System;
using System.Collections.Generic;
using System.Text;
using Booking.Domain.Email;

namespace Booking.Application.Email
{
    public interface IEmailService
    {

        Task SendBookingConfirmationToGuestAsync(
            BookingEmailDto booking,
            CancellationToken ct = default);

    }
}
