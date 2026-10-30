using System.Text.RegularExpressions;
using Copasogo.Domain.Generic.Entities;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Torneio;

namespace Copasogo.Domain.Torneio.Entities;

public sealed class Torneio : AggregateRoot
{
    private static readonly Regex SlugFormato = new("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.CultureInvariant);

    private Torneio()
    {
    }

    public string Nome { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public int Edicao { get; private set; }

    public StatusTorneio Status { get; private set; }

    public int EntryFeeCents { get; private set; }

    public int PrizePoolCents { get; private set; }

    public int QuantidadePartidas { get; private set; }

    public DateTime? IniciadoEm { get; private set; }

    public DateTime? EncerradoEm { get; private set; }

    public static Torneio Criar(string nome, string? slug, int edicao, int entryFeeCents, int prizePoolCents, int quantidadePartidas)
    {
        ValidarTaxas(entryFeeCents, prizePoolCents);
        ChaveFabrica.GarantirQuantidade(quantidadePartidas);
        if (edicao < 1)
        {
            throw new BusinessException(DomainMessages.Torneio.EdicaoInvalida);
        }

        return new Torneio
        {
            Nome = NormalizarNome(nome),
            Slug = NormalizarSlug(slug, edicao),
            Edicao = edicao,
            Status = StatusTorneio.Rascunho,
            EntryFeeCents = entryFeeCents,
            PrizePoolCents = prizePoolCents,
            QuantidadePartidas = quantidadePartidas
        };
    }

    public void AtualizarMeta(string nome, int entryFeeCents, int prizePoolCents, int quantidadePartidas)
    {
        GarantirEditavel();
        ValidarTaxas(entryFeeCents, prizePoolCents);
        ChaveFabrica.GarantirQuantidade(quantidadePartidas);
        Nome = NormalizarNome(nome);
        EntryFeeCents = entryFeeCents;
        PrizePoolCents = prizePoolCents;
        QuantidadePartidas = quantidadePartidas;
        MarkAsUpdated();
    }

    public void Iniciar()
    {
        if (Status != StatusTorneio.Rascunho)
        {
            throw new BusinessException(DomainMessages.Torneio.SoRascunhoPodeIniciar);
        }

        Status = StatusTorneio.EmAndamento;
        IniciadoEm = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Encerrar()
    {
        if (Status != StatusTorneio.EmAndamento)
        {
            throw new BusinessException(DomainMessages.Torneio.SoEmAndamentoPodeEncerrar);
        }

        Status = StatusTorneio.Encerrado;
        EncerradoEm = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void GarantirEditavel()
    {
        if (Status == StatusTorneio.Encerrado)
        {
            throw new BusinessException(DomainMessages.Torneio.EncerradoNaoPodeSerEditado);
        }
    }

    public void GarantirPodeExcluir(bool possuiPartidaTravada)
    {
        if (Status != StatusTorneio.Rascunho)
        {
            throw new BusinessException(DomainMessages.Torneio.SoRascunhoPodeSerExcluido);
        }

        if (possuiPartidaTravada)
        {
            throw new BusinessException(DomainMessages.Torneio.ComResultadoNaoPodeSerExcluido);
        }
    }

    public static string NormalizarSlug(string? slug, int edicao)
    {
        var valor = string.IsNullOrWhiteSpace(slug) ? $"copasogo-{edicao}" : slug.Trim().ToLowerInvariant();
        if (valor.Length > 80 || !SlugFormato.IsMatch(valor))
        {
            throw new BusinessException(DomainMessages.Torneio.SlugInvalido);
        }

        return valor;
    }

    private static string NormalizarNome(string nome)
    {
        var valor = nome?.Trim() ?? string.Empty;
        if (valor.Length is 0 or > 120)
        {
            throw new BusinessException(DomainMessages.Torneio.NomeObrigatorio);
        }

        return valor;
    }

    private static void ValidarTaxas(int entryFeeCents, int prizePoolCents)
    {
        if (entryFeeCents < 0)
        {
            throw new BusinessException(DomainMessages.Torneio.TaxaInvalida);
        }

        if (prizePoolCents < 0)
        {
            throw new BusinessException(DomainMessages.Torneio.BolsaInvalida);
        }
    }
}
