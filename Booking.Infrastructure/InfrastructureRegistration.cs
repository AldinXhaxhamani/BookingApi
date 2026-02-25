using Booking.Application;
using Booking.Application.Apartaments;
using Booking.Application.Users;
using Booking.Infrastructure.Apartments;
using Booking.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

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



            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            services.AddScoped<IJwtTokenService, JwtTokenService>();

            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>()!;

            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,   
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });


            services.AddAuthorization();


            return services;

        }
    }
}
