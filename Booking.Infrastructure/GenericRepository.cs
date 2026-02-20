using Booking.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Booking.Infrastructure
{
    public class GenericRepository <T> (BookingDbContext _dbContext) : IGenericRepository<T> where T : class
    {
        // ketu implementojme funksionet qe kemi ne IGeneric repository

        public async Task<T> Add(T entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }


        public async Task<T?> GetById(Guid id, CancellationToken ct)
        {
            return await _dbContext
                .Set<T>()
                .FindAsync(new object[] { id },ct);
        }

        //shtuar sepse nuk funksiononete Handle
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);

    }
}
