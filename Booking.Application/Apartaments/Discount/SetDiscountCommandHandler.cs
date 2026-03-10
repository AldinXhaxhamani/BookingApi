using Booking.Domain.Apartments;
using MediatR;


namespace Booking.Application.Apartaments.Discount
{
    public class SetDiscountCommandHandler : IRequestHandler<SetDiscountCommand>
    {

        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyAvailabilityRepository _availabilityRepository;

        public SetDiscountCommandHandler(
            IPropertyRepository propertyRepository,
            IPropertyAvailabilityRepository availabilityRepository)
        {
            _propertyRepository = propertyRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task Handle(SetDiscountCommand request, CancellationToken ct)
        {
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            if (property.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException(
                    "You do not have permission to set discounts.");

            //  one discount per property
            var existing = await _availabilityRepository
                .GetDiscountAsync(request.PropertyId, ct);

            if (existing is not null)
            {
                existing.Update(request.MinimumNights, request.DiscountPerNight);
            }
            else
            {
                var discount = PropertyDiscount.Create(
                    request.PropertyId,
                    request.MinimumNights,
                    request.DiscountPerNight);

                await _availabilityRepository.AddDiscountAsync(discount, ct);
            }

            await _availabilityRepository.SaveChangesAsync(ct);
        }

    }
}
