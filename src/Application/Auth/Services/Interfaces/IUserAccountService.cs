namespace Copasogo.Application.Auth.Services.Interfaces;

public sealed record AuthUserDto(Guid Id, string Nome, string Email);

public enum FalhaCredencial
{
    Invalidas,
    Bloqueada
}

public sealed record CredenciaisResult(bool Success, AuthUserDto? User, FalhaCredencial? Falha)
{
    public static CredenciaisResult Ok(AuthUserDto user) => new(true, user, null);

    public static CredenciaisResult Fail(FalhaCredencial falha) => new(false, null, falha);
}

public interface IUserAccountService
{
    Task<CredenciaisResult> ValidarAsync(string email, string senha, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ObterRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task EnsureRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Guid> CriarAdministradorAsync(string nome, string email, string senha, string role, CancellationToken cancellationToken = default);
}
