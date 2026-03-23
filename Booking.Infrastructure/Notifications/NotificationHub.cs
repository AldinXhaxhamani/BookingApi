using Booking.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Booking.Infrastructure.Notifications
{

    [Authorize]
    public class NotificationHub : Hub, INotificationHub
    {

        // ne momentin kur nje user behet connected, kjo method thirret automatikisht
        public override async Task OnConnectedAsync()
        {
            //merret userId nga JWT i userit qe eshte connected
            var userId = Context.User?
                .FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue("sub");

            if (!string.IsNullOrEmpty(userId))
            {
                //nese useri eshte i loguar ne me shume se 1 device, i shtoje ne nje grup te gjithe connection ID
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        // kur nje user behet disconnected kjo method thirret automatikisht
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?
                .FindFirstValue(ClaimTypes.NameIdentifier)
                ?? Context.User?.FindFirstValue("sub");

            if (!string.IsNullOrEmpty(userId))
            {
                //hiqet nga grupi,connection ID-ja e userit qe eshte disconnected
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }


    }
}
