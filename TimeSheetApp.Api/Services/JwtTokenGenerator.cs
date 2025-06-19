using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TimeSheetApp.Api.Models;

namespace TimeSheetApp.Api.Services
{
    public class JwtTokenGenerator(IConfiguration config) : IJwtTokenGenerator
    {
        public string GenerateToken(User user, IEnumerable<Claim>? additionalClaims = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject: User ID
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()), // Standard Claim for user ID
                new(ClaimTypes.Email, user.Email), // Standard Claim for email
                new(ClaimTypes.Role, user.Role) // User role
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var key = Encoding.ASCII.GetBytes(config["JwtSettings:SecretKey"]!);
            if (key == null || key.Length == 0)
            {
                throw new InvalidOperationException("JWT SecretKey is not configured or is empty.");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(config["JwtSettings:ExpiryMinutes"]!)), // Token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = config["JwtSettings:Issuer"],
                Audience = config["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}