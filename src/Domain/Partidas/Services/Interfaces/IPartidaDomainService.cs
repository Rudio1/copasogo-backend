using Copasogo.Domain.Partidas.Entities;

namespace Copasogo.Domain.Partidas.Services.Interfaces;

public interface IPartidaDomainService
{
    Task<IReadOnlyList<Partida>> ListarAsync(Guid torneioId, string? rodadaId, string? status, string? lado, CancellationToken cancellationToken = default);
    Task<Partida> ObterAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
    Task<Partida> AgendarAsync(Guid torneioId, int numero, DateTime? agendadaEm, CancellationToken cancellationToken = default);
    Task<Partida> MarcarAoVivoAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
    Task<Partida> RegistrarResultadoAsync(Guid torneioId, int numero, Guid vencedorDuplaId, int? placarA, int? placarB, CancellationToken cancellationToken = default);
    Task<Partida> CorrigirPlacarAsync(Guid torneioId, int numero, int placarA, int placarB, CancellationToken cancellationToken = default);
    Task<Partida> CancelarAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
    Task<Partida> ReabrirAsync(Guid torneioId, int numero, CancellationToken cancellationToken = default);
}
