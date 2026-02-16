using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Booking.Domain.Entities
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid GuestId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }


        [ForeignKey(nameof(GuestId))]
        public User Guest { get; set; } = null!;

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;

        public Review() { }
    }
}
