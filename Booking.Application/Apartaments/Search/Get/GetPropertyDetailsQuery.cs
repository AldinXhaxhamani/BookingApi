using Booking.Domain.Apartments.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Search.Get
{
    public class GetPropertyDetailsQuery : IRequest<PropertyDetailsDto>
    {

        public Guid PropertyId { get; set; }

    }
}
