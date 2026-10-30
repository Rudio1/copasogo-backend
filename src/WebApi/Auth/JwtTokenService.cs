using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Copasogo.Application.Auth.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Copasogo.WebApi.Auth;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public AccessTokenResult GenerateAccessToken(AuthUserDto user, IReadOnlyList<string> roles)
    {
        var signingKey = configuration["Jwt:SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("Jwt:SigningKey nao foi configurado.");
        }

        var minutes = configuration.GetValue("Jwt:AccessTokenExpirationMinutes", 60 * 24);
        var expires = DateTime.UtcNow.AddMinutes(minutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("name", user.Nome)
        };
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256));

        return new AccessTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
