using Booking.Application;
using Booking.Application.Apartaments;
using Booking.Application.Users;
using Booking.Infrastructure.Apartments;
using Booking.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Booking.Infrastructure
{
    public static class InfrastructureRegistration
    {

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("BookingConnString")));


            services.AddScoped<IApplicationContext>(
            sp => sp.GetRequiredService<BookingDbContext>());


            // Register repositorys
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();

            return services;

        }
    }
}
