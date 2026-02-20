using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Users
{
    public class CreateUserDto
    {
        public string FirstName { get; init; } //init: vlera e property vendoset kur krijohet property
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}
