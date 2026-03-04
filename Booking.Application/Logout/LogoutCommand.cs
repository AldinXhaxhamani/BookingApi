using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Logout
{
    public class LogoutCommand : IRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }

    }
}
