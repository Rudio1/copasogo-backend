using Copasogo.Application.Auth.Commands.Login;
using Copasogo.Application.Duplas.Commands;
using Copasogo.Application.Jogadores.Commands;
using Copasogo.Application.Partidas.Commands;
using Copasogo.Application.Torneio.Commands;
using Copasogo.Domain.Duplas.Services;
using Copasogo.Domain.Duplas.Services.Interfaces;
using Copasogo.Domain.Jogadores.Services;
using Copasogo.Domain.Jogadores.Services.Interfaces;
using Copasogo.Domain.Partidas.Services;
using Copasogo.Domain.Partidas.Services.Interfaces;
using Copasogo.Domain.Torneio.Services;
using Copasogo.Domain.Torneio.Services.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Copasogo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ITorneioDomainService, TorneioDomainService>();
        services.AddScoped<IJogadorDomainService, JogadorDomainService>();
        services.AddScoped<IDuplaDomainService, DuplaDomainService>();
        services.AddScoped<IPartidaDomainService, PartidaDomainService>();

        services.AddScoped<ILoginHandler, LoginHandler>();
        services.AddScoped<ICriarTorneioHandler, CriarTorneioHandler>();
        services.AddScoped<IAtualizarTorneioHandler, AtualizarTorneioHandler>();
        services.AddScoped<IIniciarTorneioHandler, IniciarTorneioHandler>();
        services.AddScoped<IEncerrarTorneioHandler, EncerrarTorneioHandler>();
        services.AddScoped<IExcluirTorneioHandler, ExcluirTorneioHandler>();
        services.AddScoped<IObterTorneioHandler, ObterTorneioHandler>();
        services.AddScoped<IObterTorneioAtualHandler, ObterTorneioAtualHandler>();
        services.AddScoped<IListarTorneiosHandler, ListarTorneiosHandler>();
        services.AddScoped<ICriarJogadorHandler, CriarJogadorHandler>();
        services.AddScoped<IAtualizarJogadorHandler, AtualizarJogadorHandler>();
        services.AddScoped<IExcluirJogadorHandler, ExcluirJogadorHandler>();
        services.AddScoped<IListarJogadoresHandler, ListarJogadoresHandler>();
        services.AddScoped<IObterJogadorHandler, ObterJogadorHandler>();
        services.AddScoped<ICriarDuplaHandler, CriarDuplaHandler>();
        services.AddScoped<IAtualizarDuplaHandler, AtualizarDuplaHandler>();
        services.AddScoped<IExcluirDuplaHandler, ExcluirDuplaHandler>();
        services.AddScoped<IListarDuplasHandler, ListarDuplasHandler>();
        services.AddScoped<IObterDuplaHandler, ObterDuplaHandler>();
        services.AddScoped<ISortearChaveHandler, SortearChaveHandler>();
        services.AddScoped<IResetarChaveHandler, ResetarChaveHandler>();
        services.AddScoped<IListarPartidasHandler, ListarPartidasHandler>();
        services.AddScoped<IObterPartidaHandler, ObterPartidaHandler>();
        services.AddScoped<IAgendarPartidaHandler, AgendarPartidaHandler>();
        services.AddScoped<IMarcarAoVivoHandler, MarcarAoVivoHandler>();
        services.AddScoped<IRegistrarResultadoHandler, RegistrarResultadoHandler>();
        services.AddScoped<ICorrigirPlacarHandler, CorrigirPlacarHandler>();
        services.AddScoped<ICancelarPartidaHandler, CancelarPartidaHandler>();
        services.AddScoped<IReabrirPartidaHandler, ReabrirPartidaHandler>();
        services.AddScoped<IConverterEmWoHandler, ConverterEmWoHandler>();

        return services;
    }
}
