using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Email
{
    public class BookingEmailDto
    {

        public string GuestEmail { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyCity { get; set; } = string.Empty;
        public string PropertyCountry { get; set; } = string.Empty;
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

    }
}
