namespace Copasogo.Application.Auth.Services.Interfaces;

public sealed record AccessTokenResult(string AccessToken, DateTime ExpiresAt);

public interface IJwtTokenService
{
    AccessTokenResult GenerateAccessToken(AuthUserDto user, IReadOnlyList<string> roles);
}
