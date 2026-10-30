using Copasogo.Application.Auth.Constants;
using Copasogo.Application.Auth.Services.Interfaces;

namespace Copasogo.WebApi.Auth;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["Admin:Email"];
        var senha = configuration["Admin:Senha"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            return;
        }

        await using var scope = services.CreateAsyncScope();
        var contas = scope.ServiceProvider.GetRequiredService<IUserAccountService>();
        await contas.EnsureRoleAsync(AuthRoles.Administrador);
        if (!await contas.ExisteEmailAsync(email))
        {
            var nome = configuration["Admin:Nome"];
            await contas.CriarAdministradorAsync(
                string.IsNullOrWhiteSpace(nome) ? "Admin" : nome,
                email,
                senha,
                AuthRoles.Administrador);
        }
    }
}
