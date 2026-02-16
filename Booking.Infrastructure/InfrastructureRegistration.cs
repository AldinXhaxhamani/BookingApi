using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure
{
    public static class InfrastructureRegistration
    {

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("BookingConnString")));

            return services;

        }
    }
}
