
using Booking.Domain.Notifications;

namespace Booking.Application.Notifications
{
    public interface INotificationService
    {

        Task NotifyBookingCreatedAsync(
            Guid guestId,
            BookingNotificationDto notification,
            CancellationToken ct = default);
    

    }
}
