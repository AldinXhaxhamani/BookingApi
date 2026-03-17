using Booking.Domain.Bookings.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Bookings.Create
{
    public class CreateBookingCommand : IRequest<CreateBookingResponseDto>
    {
        public Guid PropertyId { get; set; }  // from URL
        public Guid GuestId { get; set; }     // from JWT
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int GuestCount { get; set; }

    }
}
