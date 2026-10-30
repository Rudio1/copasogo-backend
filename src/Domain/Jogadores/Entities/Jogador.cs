using Copasogo.Domain.Generic.Entities;
using Copasogo.Domain.Generic.Validations;

namespace Copasogo.Domain.Jogadores.Entities;

public sealed class Jogador : AggregateRoot
{
    private Jogador()
    {
    }

    public string Nome { get; private set; } = string.Empty;

    public string SteamId { get; private set; } = string.Empty;

    public static Jogador Criar(string nome, string steamId)
    {
        var jogador = new Jogador();
        jogador.Atualizar(nome, steamId);
        return jogador;
    }

    public void Atualizar(string nome, string steamId)
    {
        Nome = NormalizarNome(nome);
        SteamId = NormalizarSteamId(steamId);
        MarkAsUpdated();
    }

    public static string NormalizarSteamId(string steamId)
    {
        var valor = (steamId?.Trim() ?? string.Empty).TrimEnd('/');
        if (valor.Length is < 2 or > 200 || valor.Any(char.IsWhiteSpace))
        {
            throw new BusinessException(DomainMessages.Jogador.SteamIdInvalido);
        }

        return valor;
    }

    public static string NormalizarNome(string nome)
    {
        var valor = nome?.Trim() ?? string.Empty;
        if (valor.Length is 0 or > 80)
        {
            throw new BusinessException(DomainMessages.Jogador.NomeObrigatorio);
        }

        return valor;
    }
}
