using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace Booking.Application.Apartaments.Update
{
    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IApplicationContext _applicationContext;
        private readonly UpdatePropertyCommandValidator _validator;

        public UpdatePropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IApplicationContext applicationContext)
        {
            _propertyRepository = propertyRepository;
            _applicationContext = applicationContext;
            _validator = new UpdatePropertyCommandValidator();
        }

        public async Task Handle(UpdatePropertyCommand request, CancellationToken ct)
        {
            //validojme nese i kemi te gjitha fushat e plotesuara
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            
            var property = await _propertyRepository.GetById(request.PropertyId, ct);

            if (property is null)
                throw new KeyNotFoundException("Property not found.");

            
            if (property.OwnerId != request.OwnerId)
                throw new UnauthorizedAccessException(
                    "You do not have permission to edit this property.");


            var oldAddressId = property.AddressId;

            //krijojme adresen e re 
            var newAddress = new Address(
                    request.Dto.Country,
                    request.Dto.City,
                    request.Dto.Street,
                    request.Dto.PostalCode
            );


             await _applicationContext.Addresses.AddAsync(newAddress, ct);
            

            
            property.UpdateInfo(
                request.Dto.Name,
                request.Dto.Description,
                request.Dto.PropertyType,
                request.Dto.Amenities,
                request.Dto.MaxGuests,
                request.Dto.CheckInHour,
                request.Dto.CheckOutHour,
                request.Dto.Rules,
                request.Dto.PricePerNight,
                newAddress.Id         
            );

            //gjejme dhe fshijme adresen qe property kishte me pare 
            var oldAddress = await _applicationContext.Addresses.FindAsync(new object[] { oldAddressId }, ct);

            if (oldAddress is not null)
                _applicationContext.Addresses.Remove(oldAddress);



            await _applicationContext.SaveChangesAsync(ct);
        }

    }
}
