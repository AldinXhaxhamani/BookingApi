using Booking.Domain.Apartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Register
{
    public class RegisterPropertyCommand : IRequest<Guid>
    {
        public CreatePropertyDto createPropertyDto { get; set; }
    }
}
