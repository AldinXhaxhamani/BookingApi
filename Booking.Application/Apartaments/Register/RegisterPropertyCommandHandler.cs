using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Booking.Domain.Apartments;
using Booking.Application.Apartaments;
using Booking.Application;

namespace Booking.Application.Apartaments.Register
{
    public class RegisterPropertyCommandHandler : IRequestHandler<RegisterPropertyCommand,Guid>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IApplicationContext _applicationContext;
        public RegisterPropertyCommandHandler(IPropertyRepository propertyRepository,IApplicationContext applicationContext)
        {

            _propertyRepository = propertyRepository;
            _applicationContext = applicationContext;
        }

        public async Task<Guid> Handle(
            RegisterPropertyCommand command, CancellationToken ct)
        {
            Console.WriteLine("H1: handler start");
            var propertyOwner = await _applicationContext.Owners
                .FirstOrDefaultAsync(c => c.UserId == command.createPropertyDto.OwnerId, ct);
            Console.WriteLine("H2: handler");
            if (propertyOwner == null)
            {
                Console.WriteLine("failed");
                //throw new NotFoundException("Owner not found");
            }

            var property = Property.CreateProperty(command.createPropertyDto, propertyOwner);
            Console.WriteLine("H4: property");
            await _propertyRepository.Add(property);

            Console.WriteLine("add property");
            // await _propertyRepository.SaveChangesAsync();   
            await _applicationContext.SaveChangesAsync(ct);

            Console.WriteLine("save");

            return property.Id;
        }
    }
}
