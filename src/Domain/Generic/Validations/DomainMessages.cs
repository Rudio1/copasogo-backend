namespace Copasogo.Domain.Generic.Validations;

public static class DomainMessages
{
    public static class Generic
    {
        public const string RecursoNaoEncontrado = "Recurso nao encontrado.";
        public const string OperacaoInvalida = "Operacao invalida para o estado atual.";
    }

    public static class Torneio
    {
        public const string NaoEncontrado = "Torneio nao encontrado.";
        public const string NomeObrigatorio = "O nome do torneio e obrigatorio.";
        public const string SlugInvalido = "O slug deve conter apenas letras minusculas, numeros e hifen.";
        public const string EdicaoInvalida = "A edicao deve ser maior que zero.";
        public const string EdicaoJaExiste = "Ja existe torneio com esta edicao.";
        public const string SlugJaExiste = "Ja existe torneio com este slug.";
        public const string TaxaInvalida = "A taxa de inscricao nao pode ser negativa.";
        public const string BolsaInvalida = "A bolsa nao pode ser negativa.";
        public const string JaEmAndamento = "Ja existe uma edicao em andamento.";
        public const string SoRascunhoPodeIniciar = "Somente um torneio em rascunho pode ser iniciado.";
        public const string SoEmAndamentoPodeEncerrar = "Somente um torneio em andamento pode ser encerrado.";
        public const string EncerradoNaoPodeSerEditado = "Torneio encerrado nao pode ser alterado.";
        public const string SoRascunhoPodeSerExcluido = "Somente um torneio em rascunho pode ser excluido.";
        public const string ComResultadoNaoPodeSerExcluido = "Torneio com partida ao vivo ou encerrada nao pode ser excluido.";
        public const string NenhumDisponivel = "Nenhum torneio disponivel.";
        public const string QuantidadeInvalida = "A chave aceita 3, 7, 15 ou 31 partidas.";
        public const string ChaveTravada = "A chave so pode ser refeita antes do primeiro jogo.";
        public const string ChaveCheia = "Esta chave cabe {0} duplas.";
        public const string PosicaoAcima = "Ha dupla com posicao acima do tamanho da chave.";
    }

    public static class Jogador
    {
        public const string NaoEncontrado = "Jogador nao encontrado.";
        public const string NomeObrigatorio = "O nome do jogador e obrigatorio.";
        public const string SteamIdInvalido = "Informe o SteamID64 ou o link do perfil, sem espacos.";
        public const string SteamIdJaCadastrado = "Steam ID ja cadastrado.";
        public const string JaCompetiu = "Jogador que ja entrou em partida ao vivo ou encerrada nao pode ser excluido.";
    }

    public static class Dupla
    {
        public const string NaoEncontrada = "Dupla nao encontrada.";
        public const string NomeObrigatorio = "O nome da dupla e obrigatorio.";
        public const string SeedInvalido = "A seed deve estar entre 1 e 16.";
        public const string SeedJaUsada = "Seed ja usada nesta edicao.";
        public const string ElencoInvalido = "A dupla precisa de dois jogadores distintos.";
        public const string JogadorJaNaEdicao = "Jogador ja esta em outra dupla desta edicao.";
        public const string LimiteAtingido = "Esta edicao cabe {0} duplas.";
        public const string ChaveVazia = "Gere a chave da edicao antes de sortear.";
        public const string JaJogou = "Dupla que ja entrou em partida ao vivo ou encerrada nao pode ser alterada nem excluida.";
        public const string SorteioCedo = "A chave cabe {0} duplas. Cadastre todas antes de sortear.";
        public const string SorteioTarde = "A chave so pode ser sorteada antes do primeiro jogo.";
    }

    public static class Partida
    {
        public const string NaoEncontrada = "Partida nao encontrada.";
        public const string NumeroInvalido = "O jogo informado nao existe nesta edicao.";
        public const string NaoAceitaResultado = "O jogo {0} nao aceita resultado no estado \"{1}\".";
        public const string ResultadoSemAoVivo = "Marque o jogo {0} ao vivo antes de registrar a vitoria.";
        public const string DuplaNaoJoga = "A dupla informada nao joga este jogo.";
        public const string PlacarInvalido = "O placar do jogo {0} precisa ser formado por inteiros nao negativos.";
        public const string PlacarNaoConfirma = "O placar do jogo {0} nao confirma a dupla vencedora.";
        public const string PlacarSoEncerrada = "O placar do jogo {0} so pode ser corrigido quando a partida estiver encerrada.";
        public const string SoUpcomingAoVivo = "O jogo {0} so pode ficar ao vivo quando as duas duplas estao definidas.";
        public const string EncerradaNaoCancela = "Partida encerrada nao pode ser cancelada.";
        public const string SoCanceladaReabre = "O jogo {0} so pode ser reaberto quando estiver cancelado.";
        public const string DuplaComMaisDeDuasDerrotas = "A dupla tem mais de duas derrotas.";
    }
}
