using Booking.Domain.Entities;
using Booking.Domain.Enum;
using Booking.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Apartments
{
    public class Property
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid AddressId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string PropertyType { get; set; } //kete mund ta ndryshojme ne nje enum nese duam
        public int MaxGuests { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public DateTime? LastBookedOnUtc { get; set; }


        public OwnerProfile Owner { get; set; } = null!;
        public Address Address { get; set; } = null!;




        public Property(
            Guid id,
            Guid ownerId,
            string name,
            string description,
            string propertyType , 
            Guid addressId, 
            int maxGuests, 
            DateTime checkInTime,     
            DateTime checkOutTime)
           //List<Amenity> amenities

        {
            Id = Guid.NewGuid();
            OwnerId = ownerId;
            Name = name;
            Description = description;
            PropertyType = propertyType;
            AddressId = addressId;
            MaxGuests = maxGuests;
            CheckInTime = checkInTime;
            CheckOutTime= checkOutTime;
            IsActive = true;
            IsApproved = false; //properties are not approved by default
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
            //Amenities = amenities;

        }

        public Property(
            Guid propertyId,
            Guid ownerId,
            string name,
            Guid addressId,
            string description,
            int maxGuests,
            string propertyType,
            DateTime checkInTime,
            DateTime checkOutTime,
            DateTime createdAt,
            bool isActive
            //List<Amenity> amenties
            )
        {
            Id = propertyId;
            OwnerId = ownerId;
            Name = name;
            AddressId = addressId;
            Description = description;
            MaxGuests = maxGuests;
            PropertyType = propertyType;
            CheckInTime = checkInTime;
            CheckOutTime = checkOutTime;
            CreatedAt = createdAt;
            IsActive = isActive;



            //Amenities = amentie
        }



      
        public static Property CreateProperty(CreatePropertyDto propertyDto, OwnerProfile propertyOwner)
        {

            var checkInTime = DateTime.UtcNow.Date.AddHours(14);
            var checkOutTime = DateTime.UtcNow.Date.AddDays(1).AddHours(11);
            var createdAt = DateTime.Now;
            var isActive = true;
            return new Property(
                 Guid.NewGuid(),
                 propertyOwner.UserId,
                 propertyDto.Name,
                 propertyDto.AddressId,
                 propertyDto.Description,
                 propertyDto.MaxGuests,
                 propertyDto.PropertyType,
                 checkInTime,
                 checkOutTime,
                 createdAt,
                 isActive


                //propertyDto.Amenities

                );
        }


    }
}
