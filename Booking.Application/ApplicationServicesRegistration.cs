using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MediatR;
using Booking.Application.Users.Register;
using Microsoft.Extensions.DependencyInjection;
using MediatR;


namespace Booking.Application

{
    public static class ApplicationServicesRegistration
    {


        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
           

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ApplicationServicesRegistration).Assembly));

            return services;
        }
    }
}
