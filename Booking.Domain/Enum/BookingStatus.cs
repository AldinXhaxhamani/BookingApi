using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Enum
{
    public enum BookingStatus
    {
       
        Pending = 1, //or Reserved
        Confirmed = 2,
        Rejected = 3,
        Canceled  = 4,
        Completed = 5,  
        Expired = 6,
      
    }
}
