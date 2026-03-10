using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Stay_Duration
{
    public class SetMinimumStayCommand : IRequest
    {

        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public int MinimumNights { get; set; }

    }
}
