using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Torneio;

namespace Copasogo.Domain.Jogadores.Services.Interfaces;

public sealed record JogadorConsulta(
    Jogador Jogador,
    Dupla? Dupla,
    Jogador? Parceiro,
    ClassificacaoDupla? Classificacao);

public interface IJogadorDomainService
{
    Task<Jogador> CriarAsync(string nome, string steamId, CancellationToken cancellationToken = default);
    Task<Jogador> AtualizarAsync(Guid id, string nome, string steamId, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JogadorConsulta>> ListarAsync(Guid? torneioId, CancellationToken cancellationToken = default);
    Task<JogadorConsulta> ObterAsync(Guid id, Guid? torneioId, CancellationToken cancellationToken = default);
}
