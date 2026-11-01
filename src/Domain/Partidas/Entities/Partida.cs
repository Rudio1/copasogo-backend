using Copasogo.Domain.Generic.Entities;
using Copasogo.Domain.Generic.Validations;
using Copasogo.Domain.Partidas;

namespace Copasogo.Domain.Partidas.Entities;

public sealed class Partida : AggregateRoot
{
    private Partida()
    {
    }

    public Guid TorneioId { get; private set; }

    public int Numero { get; private set; }

    public LadoChave Lado { get; private set; }

    public string RodadaId { get; private set; } = string.Empty;

    public TipoSlot SlotATipo { get; private set; }

    public Guid? SlotADuplaId { get; private set; }

    public int? SlotASeed { get; private set; }

    public int? SlotAOrigem { get; private set; }

    public TipoSlot SlotBTipo { get; private set; }

    public Guid? SlotBDuplaId { get; private set; }

    public int? SlotBSeed { get; private set; }

    public int? SlotBOrigem { get; private set; }

    public StatusPartida Status { get; private set; }

    public DateTime? AgendadaEm { get; private set; }

    public int? PlacarA { get; private set; }

    public int? PlacarB { get; private set; }

    public Guid? VencedorDuplaId { get; private set; }

    public bool Wo { get; private set; }

    public static Partida Criar(
        Guid torneioId,
        int numero,
        LadoChave lado,
        string rodadaId,
        TipoSlot tipoA,
        int? seedA,
        int? origemA,
        TipoSlot tipoB,
        int? seedB,
        int? origemB)
    {
        return new Partida
        {
            TorneioId = torneioId,
            Numero = numero,
            Lado = lado,
            RodadaId = rodadaId,
            SlotATipo = tipoA,
            SlotASeed = seedA,
            SlotAOrigem = origemA,
            SlotBTipo = tipoB,
            SlotBSeed = seedB,
            SlotBOrigem = origemB,
            Status = StatusPartida.Pending
        };
    }

    public void Agendar(DateTime? quando)
    {
        AgendadaEm = quando is null ? null : DateTime.SpecifyKind(quando.Value, DateTimeKind.Utc);
        MarkAsUpdated();
    }

    public void MarcarAoVivo()
    {
        if (Status != StatusPartida.Upcoming)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.SoUpcomingAoVivo, Numero));
        }

        Status = StatusPartida.Live;
        MarkAsUpdated();
    }

    public void RegistrarResultado(Guid vencedorDuplaId, int? placarA, int? placarB, bool wo = false)
    {
        // W.O. pode ser lançado antes do jogo começar: a dupla ausente nem chega a jogar.
        var aceita = Status == StatusPartida.Live || (wo && Status == StatusPartida.Upcoming);
        if (!aceita)
        {
            throw new BusinessException(
                Status == StatusPartida.Upcoming
                    ? string.Format(DomainMessages.Partida.ResultadoSemAoVivo, Numero)
                    : string.Format(DomainMessages.Partida.NaoAceitaResultado, Numero, StatusApi()));
        }

        VencedorDuplaId = vencedorDuplaId;
        PlacarA = wo ? null : placarA;
        PlacarB = wo ? null : placarB;
        Wo = wo;
        Status = StatusPartida.Completed;
        MarkAsUpdated();
    }

    public void CorrigirPlacar(int placarA, int placarB)
    {
        if (Status != StatusPartida.Completed || VencedorDuplaId is null)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.PlacarSoEncerrada, Numero));
        }

        // Lançar placar num jogo de W.O. desfaz o W.O.: o vencedor continua o mesmo.
        PlacarA = placarA;
        PlacarB = placarB;
        Wo = false;
        MarkAsUpdated();
    }

    // Troca o resultado de um jogo encerrado por W.O. O vencedor não muda, então a chave também não.
    public void ConverterEmWo()
    {
        if (Status != StatusPartida.Completed || VencedorDuplaId is null)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.WoSoEncerrada, Numero));
        }

        PlacarA = null;
        PlacarB = null;
        Wo = true;
        MarkAsUpdated();
    }

    public void Cancelar()
    {
        if (Status == StatusPartida.Completed)
        {
            throw new BusinessException(DomainMessages.Partida.EncerradaNaoCancela);
        }

        Status = StatusPartida.Cancelled;
        MarkAsUpdated();
    }

    // Desfaz um cancelamento. O status volta a ser calculado pela chave na sincronizacao.
    public void Reabrir()
    {
        if (Status != StatusPartida.Cancelled)
        {
            throw new BusinessException(string.Format(DomainMessages.Partida.SoCanceladaReabre, Numero));
        }

        Status = StatusPartida.Pending;
        MarkAsUpdated();
    }

    public void AplicarStatusCalculado(StatusPartida status)
    {
        if (Status is StatusPartida.Completed or StatusPartida.Live or StatusPartida.Cancelled)
        {
            return;
        }

        if (Status == status)
        {
            return;
        }

        Status = status;
        MarkAsUpdated();
    }

    public void VincularDupla(bool slotA, Guid duplaId)
    {
        if (slotA)
        {
            SlotADuplaId = duplaId;
        }
        else
        {
            SlotBDuplaId = duplaId;
        }

        MarkAsUpdated();
    }

    public void Reiniciar()
    {
        SlotADuplaId = null;
        SlotBDuplaId = null;
        Status = StatusPartida.Pending;
        AgendadaEm = null;
        PlacarA = null;
        PlacarB = null;
        VencedorDuplaId = null;
        MarkAsUpdated();
    }

    public void DesvincularDupla(bool slotA)
    {
        if (slotA)
        {
            SlotADuplaId = null;
        }
        else
        {
            SlotBDuplaId = null;
        }

        MarkAsUpdated();
    }

    public string StatusApi() => Status switch
    {
        StatusPartida.Upcoming => "upcoming",
        StatusPartida.Live => "live",
        StatusPartida.Completed => "completed",
        StatusPartida.Cancelled => "cancelled",
        _ => "pending"
    };
}
