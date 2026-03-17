using Booking.Domain.Apartments;
using Booking.Domain.Enum;
using Booking.Domain.Users;
using Booking.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Booking.Domain.Bookings
{
    public class BookingEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid GuestId { get; set; }  
 
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

        public DateTime? ExpiresAtUtc { get; set; }

       


        public Property Property { get; set; }

        [ForeignKey(nameof(GuestId))]    
        public User Guest { get; set; }

        public Booking.Domain.Reviews.Review? Review { get; set; }


     public BookingEntity(Guid propertyId,
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

        protected BookingEntity() { }



        public static BookingEntity Create(
           Guid propertyId,
           Guid guestId,
           DateTime startDate,
           DateTime endDate,
           int guestCount,
           decimal priceForPeriod,
           decimal cleaningFee,
           decimal amenitiesUpCharge,
           decimal totalPrice)
        {
            return new BookingEntity
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                GuestId = guestId,
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                GuestCount = guestCount,
                PriceForPeriod = priceForPeriod,
                CleaningFee = cleaningFee,
                AmenitiesUpCharge = amenitiesUpCharge,
                TotalPrice = totalPrice,
                BookingStatus = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(24)
            };
        }

        public void Confirm()
        {
            if (BookingStatus != BookingStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending bookings can be confirmed.");

            BookingStatus = BookingStatus.Confirmed;
            ConfirmedAtUtc = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        
        public void Reject()
        {
            if (BookingStatus != BookingStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending bookings can be rejected.");

            BookingStatus = BookingStatus.Rejected;
            RejectedAtUtc = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        
        public void Cancel()
        {
            if (BookingStatus != BookingStatus.Pending &&
                BookingStatus != BookingStatus.Confirmed)
                throw new InvalidOperationException(
                    "Only pending or confirmed bookings can be cancelled.");

            BookingStatus = BookingStatus.Canceled;
            CanceledAtUtc = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (BookingStatus != BookingStatus.Confirmed)
                throw new InvalidOperationException(
                    "Only confirmed bookings can be completed.");

            BookingStatus = BookingStatus.Completed;
            CompletedAtUtc = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        
        



    }
}
