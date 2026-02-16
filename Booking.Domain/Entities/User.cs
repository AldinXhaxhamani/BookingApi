using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Entities
{
        public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? ProfileImageUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }


        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public OwnerProfile? Owner { get; set; }


        public User (string name, string lastName, string email, string password, string phoneNumber, string? profileImageUrl)
        {
            Id = Guid.NewGuid();
            Name = name;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            ProfileImageUrl = profileImageUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt=DateTime.UtcNow;
        }




    }
}
