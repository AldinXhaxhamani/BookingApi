using Booking.Application;
using Booking.Application.Apartaments;
using Booking.Domain.Apartments;
using Booking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments.Register
{
    public class RegisterPropertyCommandHandler : IRequestHandler<RegisterPropertyCommand, Guid>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IApplicationContext _applicationContext;

        public RegisterPropertyCommandHandler(
            IPropertyRepository propertyRepository,
            IApplicationContext applicationContext)
        {
            _propertyRepository = propertyRepository;
            _applicationContext = applicationContext;
        }

        public async Task<Guid> Handle(
            RegisterPropertyCommand command,
            CancellationToken ct)
        {
            //kontrollojme nese useri eshte nje owner
            var ownerProfile = await _applicationContext.Owners
                .FirstOrDefaultAsync(o => o.UserId == command.OwnerId, ct);

            if (ownerProfile is null)
                throw new InvalidOperationException(
                    "You must complete your Owner profile before creating a property.");


            var nameIsUnique = await _propertyRepository
                .IsNameUnique(command.CreatePropertyDto.Name, ct);

            if (!nameIsUnique)
                throw new InvalidOperationException(
                    $"A property named '{command.CreatePropertyDto.Name}' already exists.");

            //krijojme adresen e property nese nuk ekziston 
            var address = await _applicationContext.Addresses
            .FirstOrDefaultAsync(a =>
           a.Country == command.CreatePropertyDto.Country &&
           a.City == command.CreatePropertyDto.City &&
           a.Street == command.CreatePropertyDto.Street &&
           a.PostalCode == command.CreatePropertyDto.PostalCode, ct);

            if (!(address is null))
            {
                throw new InvalidOperationException(
                    $"This address already exists.");
                
            }
            address = new Address(
                    command.CreatePropertyDto.Country,
                    command.CreatePropertyDto.City,
                    command.CreatePropertyDto.Street,
                    command.CreatePropertyDto.PostalCode
                );
            await _applicationContext.Addresses.AddAsync(address, ct);
            

            

            
            var property = Property.CreateProperty(
                command.CreatePropertyDto,
                command.OwnerId,
                address.Id
            );

            await _propertyRepository.Add(property);

            
            await _applicationContext.SaveChangesAsync(ct);

            return property.Id;
        }
    }
}
