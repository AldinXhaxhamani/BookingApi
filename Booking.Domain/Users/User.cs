using Booking.Domain.Apartments;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Users
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
        public bool IsActive { get; private set; }= true;
        public DateTime CreatedAt { get; private set; }=DateTime.Now;
        public DateTime? LastModifiedAt { get; private set; }


        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public OwnerProfile? Owner { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();


        public User(Guid id, string firstName, string lastName, string email, string password,string phoneNumber, DateTime createdAt)
        {
            Id = id;
            this.Name = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            PhoneNumber = phoneNumber;
            this.CreatedAt = createdAt;
        }

        private User() { }

        public static User CreateUser(CreateUserDto userDto)
        {
            Guid id = Guid.NewGuid();

            DateTime data = DateTime.UtcNow;

            return new User(id, userDto.FirstName, userDto.LastName,
                userDto.Email, userDto.Password,userDto.PhoneNumber, data);
        }



    }
}
