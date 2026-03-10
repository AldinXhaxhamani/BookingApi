using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Stay_Duration
{
    public class SetMinimumStayCommandHandler : IRequestHandler<SetMinimumStayCommand>
    {

        private readonly IPropertyRepository _propertyRepository;

        public SetMinimumStayCommandHandler(IPropertyRepository propertyRepository)
            => _propertyRepository = propertyRepository;

        public async Task Handle(SetMinimumStayCommand request, CancellationToken ct)
        {
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            if (property.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException(
                    "You do not have permission to update this property.");

           
            property.SetMinimumStay(request.MinimumNights);
            await _propertyRepository.SaveChangesAsync(ct);
        }

    }
}
