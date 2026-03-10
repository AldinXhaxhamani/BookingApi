using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Apartments
{
    public class PropertyDiscount
    {

        [Key]
        public Guid Id { get; private set; }

        public Guid PropertyId { get; private set; }

        public int MinimumNights { get; private set; }

        public decimal DiscountPerNight { get; private set; }

        public Property Property { get; set; } = null!;

        private PropertyDiscount() { }

        public static PropertyDiscount Create(
            Guid propertyId, int minimumNights, decimal discountPerNight)
        {
            if (minimumNights < 2)
                throw new InvalidOperationException(
                    "Minimum nights must be at least 2.");

            if (discountPerNight <= 0)
                throw new InvalidOperationException(
                    "Discount per night must be greater than 0.");

            return new PropertyDiscount
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                MinimumNights = minimumNights,
                DiscountPerNight = discountPerNight
            };
        }

        public void Update(int minimumNights, decimal discountPerNight)
        {
            if (minimumNights < 2)
                throw new InvalidOperationException(
                    "Minimum nights must be at least 2.");

            if (discountPerNight <= 0)
                throw new InvalidOperationException(
                    "Discount per night must be greater than 0.");

            MinimumNights = minimumNights;
            DiscountPerNight = discountPerNight;
        }

        public decimal Calculate(int numberOfNights)
        {
            if (numberOfNights < MinimumNights)
                return 0;

            return numberOfNights * DiscountPerNight;
        }

    }
}
