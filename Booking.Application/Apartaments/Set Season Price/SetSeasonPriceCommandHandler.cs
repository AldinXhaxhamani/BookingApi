using Booking.Application.Apartaments.NewFolder;
using Booking.Domain.Apartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Set_Season_Price
{
    public class SetSeasonPriceCommandHandler: IRequestHandler<SetSeasonPriceCommand>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyAvailabilityRepository _availabilityRepository;

        public SetSeasonPriceCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyAvailabilityRepository availabilityRepository)
        {
            _propertyRepository = propertyRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task Handle(SetSeasonPriceCommand request, CancellationToken ct)
        {
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            if (property.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException(
                    "You do not have permission to set season prices.");

            //update if exists, create if not
            var existing = await _availabilityRepository
                .GetSeasonPriceAsync(request.PropertyId, request.Season, ct);

            if (existing is not null)
            {
                existing.UpdatePrice(request.PricePerNight);
            }
            else
            {
                var seasonPrice = PropertySeasonPrice.Create(
                    request.PropertyId,
                    request.Season,
                    request.PricePerNight);

                await _availabilityRepository.AddSeasonPriceAsync(seasonPrice, ct);
            }

            await _availabilityRepository.SaveChangesAsync(ct);
        }

    }
}
