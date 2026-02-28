using Booking.Application;
using Booking.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



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
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }




        private SigningCredentials GetSigningCredentials()
        {
            
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var secret = new SymmetricSecurityKey(key);

           
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

       


        private static List<Claim> GetClaims(User user)
        {
          
            var userRoles = user.UserRoles
                .Select(ur => ur.Role)
                .ToList();

            var claims = new List<Claim>
            {
            
                 new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            
            };


            claims.AddRange(userRoles
                .Select(role => new Claim("role", role.Name)));


            return claims;
        }

        
        private JwtSecurityToken GenerateTokenOptions(
            SigningCredentials signingCredentials,
            List<Claim> claims)
        {
            return new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: signingCredentials
            );
        }
    }
}
