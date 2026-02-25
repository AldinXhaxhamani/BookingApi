using Booking.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Booking.Domain.Entities
{
    public class OwnerProfile
    {
        [Key]
        public Guid UserId { get; set; }
        public string IdentityCardNumber { get; set; }
        public bool VerificationStatus { get; set; }=false;
        public string? BusinessName { get; set; }
        public string? CreditCard { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public DateTime? LastModifiedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
       


        public OwnerProfile(Guid userId,string identityCardNumber, bool verificationStatus, string? businessName, string creditCard)
        {
            UserId= userId;
            IdentityCardNumber = identityCardNumber;
            VerificationStatus = verificationStatus;
            BusinessName = businessName;
            CreditCard = creditCard;
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        private OwnerProfile() { }
    }
}
