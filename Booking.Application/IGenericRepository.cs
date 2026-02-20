using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Application
{
    public interface IGenericRepository <T> where T : class
    {
        Task <T> Add(T entity);
        void Delete (T entity);
        void Update (T entity);
        Task <T?> GetById(Guid id, CancellationToken ct);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
