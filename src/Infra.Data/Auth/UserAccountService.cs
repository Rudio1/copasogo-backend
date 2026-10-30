using Copasogo.Application.Auth.Services.Interfaces;
using Copasogo.Infra.Data.Auth.Identity;
using Microsoft.AspNetCore.Identity;

namespace Copasogo.Infra.Data.Auth;

public sealed class UserAccountService(
    UserManager<ApplicationUser> users,
    RoleManager<IdentityRole<Guid>> roles) : IUserAccountService
{
    public async Task<CredenciaisResult> ValidarAsync(string email, string senha, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByEmailAsync(email.Trim());
        if (user is null)
        {
            return CredenciaisResult.Fail(FalhaCredencial.Invalidas);
        }

        if (await users.IsLockedOutAsync(user))
        {
            return CredenciaisResult.Fail(FalhaCredencial.Bloqueada);
        }

        if (!await users.CheckPasswordAsync(user, senha))
        {
            await users.AccessFailedAsync(user);
            return await users.IsLockedOutAsync(user)
                ? CredenciaisResult.Fail(FalhaCredencial.Bloqueada)
                : CredenciaisResult.Fail(FalhaCredencial.Invalidas);
        }

        await users.ResetAccessFailedCountAsync(user);
        return CredenciaisResult.Ok(new AuthUserDto(user.Id, user.Nome, user.Email ?? email.Trim()));
    }

    public async Task<IReadOnlyList<string>> ObterRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await users.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return [];
        }

        return (await users.GetRolesAsync(user)).ToArray();
    }

    public async Task EnsureRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        if (!await roles.RoleExistsAsync(role))
        {
            await roles.CreateAsync(new IdentityRole<Guid>(role) { Id = Guid.NewGuid() });
        }
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await users.FindByEmailAsync(email.Trim()) is not null;
    }

    public async Task<Guid> CriarAdministradorAsync(string nome, string email, string senha, string role, CancellationToken cancellationToken = default)
    {
        var normalizado = email.Trim();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = normalizado,
            Email = normalizado,
            EmailConfirmed = true,
            Nome = nome.Trim()
        };

        var criado = await users.CreateAsync(user, senha);
        if (!criado.Succeeded)
        {
            throw new InvalidOperationException(string.Join("; ", criado.Errors.Select(error => error.Description)));
        }

        await users.AddToRoleAsync(user, role);
        return user.Id;
    }
}
