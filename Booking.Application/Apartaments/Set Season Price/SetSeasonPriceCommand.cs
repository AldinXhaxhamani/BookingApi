using Booking.Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.NewFolder
{
    public class SetSeasonPriceCommand : IRequest
    {
        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public Season Season { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
