using Booking.Application;
using Booking.Domain.Users;
using Microsoft.Extensions.Options;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace Booking.Infrastructure
{
    public class JwtTokenService : IJwtTokenService
    {

        public readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user)
        {
            //throw new NotImplementedException();

            var header= new Dictionary<string, string>()
            {
                { "alg", "HS256" },
                { "typ", "JWT" }
            };

             var nowTime= DateTimeOffset.UtcNow;

            var roles= user.UserRoles
                .Select(ur => ur.Role.Name)
                .ToArray();

            var payload = new Dictionary<string, object>
            {
                ["sub"] = user.Id.ToString(),                          // user id
                ["role"] = roles.Length == 1 ? roles[0] : roles,      // single string or array
                ["exp"] = nowTime.AddMinutes(_jwtSettings.ExpirationMinutes)  // expiry(shtojme kohen we kemi tek json settings)
                        .ToUnixTimeSeconds(),
            };


            var headerEncoded = Base64Url.EncodeToString(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header))

              );

             var payloadEncoded = Base64Url.EncodeToString(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload))
                );


            var signingInput = $"{headerEncoded}.{payloadEncoded}";

           
            var signatureBytes = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey),  
                Encoding.UTF8.GetBytes(signingInput)          
            );

            var signature = Base64Url.EncodeToString(signatureBytes);

          
            return $"{signingInput}.{signature}";


        }
    }
}
