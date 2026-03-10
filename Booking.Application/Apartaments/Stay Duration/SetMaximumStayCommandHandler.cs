using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Booking.Application.Apartaments.Stay_Duration
{
    public class SetMaximumStayCommandHandler : IRequestHandler<SetMaximumStayCommand>
    {

        private readonly IPropertyRepository _propertyRepository;

        public SetMaximumStayCommandHandler(IPropertyRepository propertyRepository)
            => _propertyRepository = propertyRepository;

        public async Task Handle(SetMaximumStayCommand request, CancellationToken ct)
        {
            var property = await _propertyRepository
                .GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            if (property.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException(
                    "You do not have permission to update this property.");

            
            property.SetMaximumStay(request.MaximumNights);
            await _propertyRepository.SaveChangesAsync(ct);
        }

    }
}
