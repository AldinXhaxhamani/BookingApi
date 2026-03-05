using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Users
{
    public class UpdateUserDto
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
