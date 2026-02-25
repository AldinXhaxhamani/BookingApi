using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<UserRole> UserRoles  { get; set; } = new List<UserRole>(); //nje rol ka shume users
       // public UserRole UserRole { get; set; }//eshte gabim. ne vend te kesaj duhet lista. lidhje many to many me tabelen qe kemi role--user 

        public Role(string name, string description) //name thirret si RoleEnum.Admin, RoleEnum.User, RoleEnum.Guest
        { 

            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Description = description;
        
        }

    }
}
