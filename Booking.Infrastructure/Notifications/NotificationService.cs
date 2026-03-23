using Booking.Domain.Notifications;
using Microsoft.AspNetCore.SignalR;
using Booking.Application.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Infrastructure.Notifications
{
    public  class NotificationService : INotificationService
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyBookingCreatedAsync(
            Guid guestId,
            BookingNotificationDto notification,
            CancellationToken ct = default)
        {
            await _hubContext.Clients
                .Group(guestId.ToString())
                .SendAsync("BookingCreated", notification, ct);
        }

    }
}
