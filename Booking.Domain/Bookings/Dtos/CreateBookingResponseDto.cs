using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Bookings.Dtos
{
    public  class CreateBookingResponseDto
    {

        public Guid BookingId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int GuestCount { get; set; }



        
        public decimal PriceForPeriod { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal AmenitiesUpCharge { get; set; }
        public decimal ServiceFee { get; set; }   
        public decimal TaxAmount { get; set; }    
        public decimal TotalPrice { get; set; }

        public DateTime ExpiresAtUtc { get; set; }
        public string Message { get; set; } = string.Empty;

    }
}
