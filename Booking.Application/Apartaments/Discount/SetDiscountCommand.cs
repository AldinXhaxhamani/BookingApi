using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Discount
{
    public class SetDiscountCommand : IRequest
    {

        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public int MinimumNights { get; set; }
        public decimal DiscountPerNight { get; set; }

    }
}
