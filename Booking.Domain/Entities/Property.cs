using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Entities
{
    public class Property
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid AddressId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PropertyType { get; set; } //kete mund ta ndryshojme ne nje enum nese duam
        public int MaxGuests { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public DateTime LastBookedOnUtc { get; set; }



        public OwnerProfile Owner { get; set; }
        public Address Address { get; set; }



        public Property(Guid ownerId, string name, string description, string propertyType, Guid addressId, int maxGuests, DateTime checkInTime, DateTime checkOutTime)
        {
            Id = Guid.NewGuid();
            OwnerId = ownerId;
            Name = name;
            Description = description;
            PropertyType = propertyType;
            AddressId = addressId;
            MaxGuests = maxGuests;
            CheckInTime = checkInTime;
            CheckOutTime = checkOutTime;
            IsActive = true;
            IsApproved = false; //properties are not approved by default
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }




    }
}
