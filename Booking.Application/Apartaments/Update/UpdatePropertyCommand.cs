using Booking.Domain.Apartments;
using Booking.Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Update
{
    public class UpdatePropertyCommand : IRequest
    {
        public Guid PropertyId { get; set; }           // e marrim nga URL  kur therrasim API
        public Guid OwnerId { get; set; }             
        public CreatePropertyDto Dto { get; set; } = new(); // reuse  DTO e create


    }
}
