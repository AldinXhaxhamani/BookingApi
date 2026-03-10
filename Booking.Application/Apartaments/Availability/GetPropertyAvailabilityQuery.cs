using Booking.Domain.Apartments.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Availability
{
    public class GetPropertyAvailabilityQuery : IRequest<PropertyAvailabilityDto>
    {

        public Guid PropertyId { get; set; }


    }
}
