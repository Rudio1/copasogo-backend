using Copasogo.Application.Auth.Services.Interfaces;
using Copasogo.Domain.Duplas.Repositories.Interfaces;
using Copasogo.Domain.Generic.Repositories.Interfaces;
using Copasogo.Domain.Jogadores.Repositories.Interfaces;
using Copasogo.Domain.Partidas.Repositories.Interfaces;
using Copasogo.Domain.Torneio.Repositories.Interfaces;
using Copasogo.Infra.Data.Auth;
using Copasogo.Infra.Data.Auth.Identity;
using Copasogo.Infra.Data.Context;
using Copasogo.Infra.Data.Duplas.Repositories;
using Copasogo.Infra.Data.Generic.Repositories;
using Copasogo.Infra.Data.Jogadores.Repositories;
using Copasogo.Infra.Data.Partidas.Repositories;
using Copasogo.Infra.Data.Torneio.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Copasogo.Infra.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' nao configurada. " +
                "Defina em appsettings.Development.json ou na variavel de ambiente correspondente.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<ITorneioRepository, TorneioRepository>();
        services.AddScoped<IJogadorRepository, JogadorRepository>();
        services.AddScoped<IDuplaRepository, DuplaRepository>();
        services.AddScoped<IPartidaRepository, PartidaRepository>();

        return services;
    }
}
