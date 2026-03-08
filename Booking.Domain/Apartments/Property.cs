using Booking.Domain.Entities;
using Booking.Domain.Enum;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace Booking.Domain.Apartments
{
    public class Property
    {

        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid AddressId { get; set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        //ruhen si int ne DB — enum in code 
        public PropertyType PropertyType { get; private set; }

        
        public string? AmenitiesRaw { get; private set; }

        public int MaxGuests { get; private set; }
        public DateTime CheckInTime { get; private set; }
        public DateTime CheckOutTime { get; private set; }
        public string? Rules { get; private set; }
        public decimal PricePerNight { get; private set; }
        public string? PhotoUrl { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsApproved { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastModifiedAt { get; private set; }
        public DateTime? LastBookedOnUtc { get; set; }
        public OwnerProfile Owner { get; set; } = null!;
        public Address Address { get; set; } = null!;

        


        public List<Amenity> Amenities =>
            string.IsNullOrEmpty(AmenitiesRaw)
                ? new List<Amenity>()
                : AmenitiesRaw
                    .Split(',')
                    .Select(a => (Amenity)int.Parse(a))
                    .ToList();

        private Property() { }

        public static Property CreateProperty(
            CreatePropertyDto dto,
            Guid ownerId,
            Guid addressId)
        {
            return new Property
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                AddressId = addressId,
                Name = dto.Name,
                Description = dto.Description,
                PropertyType = dto.PropertyType,
                AmenitiesRaw = string.Join(",",
                    dto.Amenities.Select(a => (int)a)), // [WiFi,Parking] → "1,3"
                MaxGuests = dto.MaxGuests,
                CheckInTime = DateTime.UtcNow.Date.AddHours(dto.CheckInHour),
                CheckOutTime = DateTime.UtcNow.Date.AddHours(dto.CheckOutHour),
                Rules = dto.Rules,
                PricePerNight = dto.PricePerNight,
                IsActive = true,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };
        }

        public void UpdateInfo(
            string name,
            string? description,
            PropertyType propertyType,
            List<Amenity> amenities,
            int maxGuests,
            int checkInHour,
            int checkOutHour,
            string? rules,
            decimal pricePerNight,
            Guid addressId)
        {
            Name = name;
            Description = description;
            PropertyType = propertyType;
            AmenitiesRaw = string.Join(",", amenities.Select(a => (int)a));
            MaxGuests = maxGuests;
            CheckInTime = DateTime.UtcNow.Date.AddHours(checkInHour);
            CheckOutTime = DateTime.UtcNow.Date.AddHours(checkOutHour);
            Rules = rules;
            PricePerNight = pricePerNight;
            LastModifiedAt = DateTime.UtcNow;
            AddressId= addressId;
        }

        public void UpdatePhoto(string photoUrl)
        {
            PhotoUrl = photoUrl;
            LastModifiedAt = DateTime.UtcNow;
        }
    }
}
