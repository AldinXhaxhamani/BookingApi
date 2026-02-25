using Booking.Domain.Apartments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments
{
    public interface IPropertyRepository: IGenericRepository<Property>
    {
        Task<bool> IsNameUnique(string name, CancellationToken cancellationToken);
    }
}
