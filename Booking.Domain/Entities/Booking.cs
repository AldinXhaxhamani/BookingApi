using Booking.Domain.Apartments;
using Booking.Domain.Enum;
using Booking.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Booking.Domain.Entities
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid GuestId { get; set; }  
        //public Guid? ReviewId { get; set; }
        public  DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int GuestCount { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal AmenitiesUpCharge { get; set; }
        public decimal PriceForPeriod { get; set; }  
        public decimal TotalPrice     { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public DateTime CreatedAt   { get; set; }
        public DateTime LastModifiedAt  { get; set; }
        public DateTime CreatedAtUtc    { get; set; }
        public DateTime? ConfirmedAtUtc { get; set; }
        public DateTime? RejectedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime? CanceledAtUtc { get; set; }


       // public List<>//Lista e amenties, enum i krijuar


        public Property Property { get; set; }

        [ForeignKey(nameof(GuestId))]    
        public User Guest { get; set; }

     
        public Review? Review { get; set; }


     public Booking(Guid propertyId,
        Guid guestId,
        DateTime startDate,
        DateTime endDate,
        int guestCount,
        decimal cleaningFee,
        decimal amenitiesUpCharge,
        decimal priceForPeriod,
        decimal totalPrice)

        {
            Id = Guid.NewGuid();   
            PropertyId = propertyId;
            GuestId = guestId;
            StartDate = startDate;
            EndDate = endDate;
            GuestCount = guestCount;
            CleaningFee = cleaningFee;
            AmenitiesUpCharge = amenitiesUpCharge;
            PriceForPeriod = priceForPeriod;
            TotalPrice = totalPrice;
            BookingStatus = BookingStatus.Pending;
            CreatedAtUtc = DateTime.UtcNow;
        }

        protected Booking() { }


    }
}
