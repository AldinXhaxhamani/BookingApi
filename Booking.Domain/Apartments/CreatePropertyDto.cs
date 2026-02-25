using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Apartments
{
    public class CreatePropertyDto
    {
        public Guid OwnerId { get; set; }
        public string Name { get; init; }
        public Guid AddressId { get; init; }
        public string? Description { get; set;}
        public int MaxGuests { get; init; }
        public string PropertyType { get; init; }
       

    }
}
