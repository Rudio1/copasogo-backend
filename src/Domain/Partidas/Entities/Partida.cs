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

    public void RegistrarResultado(Guid vencedorDuplaId, int? placarA, int? placarB)
    {
        if (Status != StatusPartida.Live)
        {
            throw new BusinessException(
                Status == StatusPartida.Upcoming
                    ? string.Format(DomainMessages.Partida.ResultadoSemAoVivo, Numero)
                    : string.Format(DomainMessages.Partida.NaoAceitaResultado, Numero, StatusApi()));
        }

        VencedorDuplaId = vencedorDuplaId;
        PlacarA = placarA;
        PlacarB = placarB;
        Status = StatusPartida.Completed;
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
