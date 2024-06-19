using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface ITokenService
{
    public (string token, DateTime expiration) GenerateRefreshToken();
    public string GenerateAccessToken(User user);
    public ClaimsPrincipal? ValidateAndGetPrincipalFromJwt(string token, bool validateLifetime = true);
}

public class TokenService : ITokenService
{
    public (string token, DateTime expiration) GenerateRefreshToken()
    {
        // Should come from app-settings
        return (Guid.NewGuid().ToString(), DateTime.UtcNow.AddHours(24));
    }
    
    public string GenerateAccessToken(User user)
    {
        var userClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        // Should come from app-settings
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("e5be8f13-627b-4632-805f-37a86ce0d76d"));

        // Should come from app-settings
        var token = new JwtSecurityToken(
            claims: userClaims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateAndGetPrincipalFromJwt (string token, bool validateLifetime = true)
    {
        try
        {
            // Should come from app-settings
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("e5be8f13-627b-4632-805f-37a86ce0d76d")),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            };
            
            ClaimsPrincipal? principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, tokenValidationParameters, out var securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}