using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Torneio;

namespace Copasogo.Domain.Duplas.Services.Interfaces;

public sealed record DuplaConsulta(
    Dupla Dupla,
    Jogador JogadorA,
    Jogador JogadorB,
    ClassificacaoDupla Classificacao,
    CaminhoDupla Caminho);

public interface IDuplaDomainService
{
    Task<DuplaConsulta> CriarAsync(Guid torneioId, string nome, Guid jogadorAId, Guid jogadorBId, CancellationToken cancellationToken = default);
    Task<DuplaConsulta> AtualizarAsync(Guid id, string nome, Guid jogadorAId, Guid jogadorBId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DuplaConsulta>> SortearAsync(Guid torneioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DuplaConsulta>> ResetarChaveAsync(Guid torneioId, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DuplaConsulta>> ListarAsync(Guid torneioId, CancellationToken cancellationToken = default);
    Task<DuplaConsulta> ObterAsync(Guid id, CancellationToken cancellationToken = default);
}
