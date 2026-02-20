using Booking.Application;
using Booking.Application.Users;
using Booking.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
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


            //shtim pas run-imit **
            services.AddScoped<IApplicationContext>(sp =>
           sp.GetRequiredService<IApplicationContext>());

            // Register repository
            services.AddScoped<IUserRepository, UserRepository>();

            return services;

        }
    }
}
