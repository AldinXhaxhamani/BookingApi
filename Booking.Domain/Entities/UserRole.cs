using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; }


        public User User { get; set; } 
        public Role Role { get; set; }


        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
            AssignedAt = DateTime.UtcNow;
        }

        protected UserRole() { }
    }
}
