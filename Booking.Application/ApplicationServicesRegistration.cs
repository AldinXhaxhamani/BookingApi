using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Booking.Application

{
    public static class ApplicationServicesRegistration
    {


        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
          
            return services;
        }
    }
}
