using Copasogo.Domain.Generic.Entities;
using Copasogo.Domain.Generic.Validations;

namespace Copasogo.Domain.Duplas.Entities;

public sealed class Dupla : AggregateRoot
{
    private Dupla()
    {
    }

    public Guid TorneioId { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public int? Seed { get; private set; }

    public Guid JogadorAId { get; private set; }

    public Guid JogadorBId { get; private set; }

    public static Dupla Criar(Guid torneioId, string nome, Guid jogadorAId, Guid jogadorBId)
    {
        var dupla = new Dupla { TorneioId = torneioId };
        dupla.Redefinir(nome, jogadorAId, jogadorBId);
        return dupla;
    }

    public void DefinirSeed(int seed, int vagas)
    {
        if (seed < 1 || seed > vagas)
        {
            throw new BusinessException(DomainMessages.Dupla.SeedInvalido);
        }

        Seed = seed;
        MarkAsUpdated();
    }

    public void LimparSeed()
    {
        if (Seed is null)
        {
            return;
        }

        Seed = null;
        MarkAsUpdated();
    }

    public void Redefinir(string nome, Guid jogadorAId, Guid jogadorBId)
    {
        if (jogadorAId == Guid.Empty || jogadorBId == Guid.Empty || jogadorAId == jogadorBId)
        {
            throw new BusinessException(DomainMessages.Dupla.ElencoInvalido);
        }

        var valor = nome?.Trim() ?? string.Empty;
        if (valor.Length is 0 or > 80)
        {
            throw new BusinessException(DomainMessages.Dupla.NomeObrigatorio);
        }

        Nome = valor;
        JogadorAId = jogadorAId;
        JogadorBId = jogadorBId;
        MarkAsUpdated();
    }

    public bool PossuiJogador(Guid jogadorId) => JogadorAId == jogadorId || JogadorBId == jogadorId;
}
