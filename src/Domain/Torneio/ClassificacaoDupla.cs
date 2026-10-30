using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Domain.Partidas.Entities;

namespace Copasogo.Domain.Torneio;

public sealed record ClassificacaoDupla(Guid DuplaId, string Estado, int Derrotas);

public sealed record CaminhoDupla(IReadOnlyList<int> Passado, IReadOnlyList<int> Proximo);

public sealed record VisaoTorneio(
    Entities.Torneio Torneio,
    IReadOnlyList<Jogador> Jogadores,
    IReadOnlyList<Dupla> Duplas,
    IReadOnlyList<Partida> Partidas,
    IReadOnlyList<ClassificacaoDupla> Classificacao,
    Guid? CampeaoDuplaId,
    IReadOnlyDictionary<Guid, CaminhoDupla> Caminhos);

public sealed record ResumoTorneio(Entities.Torneio Torneio, Guid? CampeaoDuplaId, string? CampeaoNome);
