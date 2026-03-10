using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Stay_Duration
{
    public class SetMaximumStayCommand : IRequest
    {
        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public int MaximumNights { get; set; }
    }
}
