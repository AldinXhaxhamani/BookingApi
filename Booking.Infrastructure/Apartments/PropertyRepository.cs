using Booking.Domain.Apartments;
using Booking.Application.Apartaments;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Apartments
{
    public class PropertyRepository : GenericRepository <Property>,IPropertyRepository
    {
        private readonly BookingDbContext _dbContext;

        public PropertyRepository(BookingDbContext dbContext) : base (dbContext) {
        
            _dbContext = dbContext;
        }

        public async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
        {
            var isUnique=await _dbContext.Properties
                .Where(c=> c.Name==name).ToListAsync(cancellationToken);
            return isUnique.Count == 0; //meqe me siper na kthehet nje lsite me propertioes me kte emer 
                
        }

           
    }
}
