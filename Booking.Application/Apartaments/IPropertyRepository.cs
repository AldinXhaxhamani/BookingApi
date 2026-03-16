using Booking.Application.Apartaments.Filter_Properties;
using Booking.Application.Apartaments.Search;
using Booking.Domain.Apartments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application.Apartaments
{
    public interface IPropertyRepository: IGenericRepository<Property>
    {
        Task<bool> IsNameUnique(string name, CancellationToken cancellationToken);

        Task<List<PropertySearchProjection>> SearchAsync(
        PropertySearchFilter filter,
        CancellationToken ct = default);

        Task<(List<PropertySearchProjection> Items, int TotalCount)> SearchAsync(
            PropertySearchFilter filter,
            int page,
            int pageSize,
            CancellationToken ct = default);
    }
}
