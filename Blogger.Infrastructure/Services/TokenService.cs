using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blogger.Contracts.Configs;
using Blogger.Contracts.Data.Entities;
using Blogger.Contracts.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Blogger.Infrastructure.Services;

public class TokenService(IOptions<JwtTokenConfig> tokenConfig)
    : ITokenService
{
    public async Task<string> GenerateAsync(ApplicationUser applicationUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(tokenConfig.Value.ExpiryInMinutes),
            Issuer = tokenConfig.Value.Issuer,
            Audience = tokenConfig.Value.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.Value.Key)),
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

       return tokenHandler.WriteToken(token);
    }
}