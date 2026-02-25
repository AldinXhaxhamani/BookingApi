using Booking.Domain.Apartments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Entities
{
    public class Address

    {
        [Key]
        public Guid Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        public List<Property> Properties { get; set; }=new List<Property>();



       public Address(
       string country,
       string city,
       string street,
       string postalCode)
        {
            Id = Guid.NewGuid();
            Country = country;
            City = city;
            Street = street;
            PostalCode = postalCode;
        }




    }
}
