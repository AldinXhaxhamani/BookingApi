using Booking.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Booking.Domain.Apartments
{
    public class PropertySeasonPrice
    {

        [Key]
        public Guid Id { get; private set; }

        public Guid PropertyId { get; private set; }

        public Season Season { get; private set; }

        public decimal PricePerNight { get; private set; }



        public Property Property { get; set; } = null!;

        private PropertySeasonPrice() { }

        public static PropertySeasonPrice Create(
            Guid propertyId, Season season, decimal pricePerNight)
        {
            if (pricePerNight <= 0)
                throw new InvalidOperationException(
                    "Price per night must be greater than 0.");

            return new PropertySeasonPrice
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                Season = season,
                PricePerNight = pricePerNight
            };
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new InvalidOperationException(
                    "Price per night must be greater than 0.");

            PricePerNight = newPrice;
        }

    }
}
