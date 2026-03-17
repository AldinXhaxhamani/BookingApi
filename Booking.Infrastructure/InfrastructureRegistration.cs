using Booking.Application;
using Booking.Application.Apartaments;
using Booking.Application.Bookings;
using Booking.Application.Email;
using Booking.Application.Roles;
using Booking.Application.Users;
using Booking.Infrastructure.Apartments;
using Booking.Infrastructure.Bookings;
using Booking.Infrastructure.Email;
using Booking.Infrastructure.Roles;
using Booking.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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
            
            services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IPropertyAvailabilityRepository, PropertyAvailabilityRepository>();

            services.AddScoped<IBookingRepository, BookingRepository>();

            
            services.Configure<SendGridSettings>(
                configuration.GetSection("SendGrid"));

            
            services.AddScoped<IEmailService, SendGridEmailService>();




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
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        RoleClaimType = "role"

                    };

                    options.Events = new JwtBearerEvents
                    {

                        OnTokenValidated = async context =>
                        {
                            var blacklistRepo = context.HttpContext.RequestServices
                                .GetRequiredService<ITokenBlacklistRepository>();

                            var token = context.HttpContext.Request.Headers["Authorization"]
                                .ToString().Replace("Bearer ", "");

                            var isBlacklisted = await blacklistRepo.IsBlacklistedAsync(token);

                            if (isBlacklisted)
                                context.Fail("Token has been invalidated. Please login again.");



                            //kontrollojme nese Useri qe mund te kete tokenin aktiv eshte fshire
                            var userRepo = context.HttpContext.RequestServices
                                .GetRequiredService<IUserRepository>();

                            var userIdClaim = context.Principal?.FindFirstValue("sub");

                            if (userIdClaim is not null && Guid.TryParse(userIdClaim, out var userId))
                            //out var userId, krijo nje variabel userId nese parsimi eshte i suksesshem
                            {
                                var user = await userRepo.GetByIdWithRolesAsync(userId);

                                if (user is not null && !user.IsActive)
                                    context.Fail("Your account has been deactivated.");
                            }

                        },

                    };
                });






            services.AddAuthorization();


            return services;

        }
    }
}
