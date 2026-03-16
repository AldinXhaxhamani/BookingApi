using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Review
{
    public class ReviewDto
    {

        public Guid Id { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
