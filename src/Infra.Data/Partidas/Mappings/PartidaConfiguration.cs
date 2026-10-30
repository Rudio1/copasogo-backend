using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Partidas.Entities;
using Copasogo.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Infra.Data.Partidas.Mappings;

public sealed class PartidaConfiguration : IEntityTypeConfiguration<Partida>
{
    public void Configure(EntityTypeBuilder<Partida> builder)
    {
        builder.ToTable("partidas");
        builder.ConfigureAuditColumns();
        builder.Property(partida => partida.TorneioId).HasColumnName("torneio_id");
        builder.Property(partida => partida.Numero).HasColumnName("numero");
        builder.Property(partida => partida.Lado).HasColumnName("lado").HasConversion<string>().HasMaxLength(20);
        builder.Property(partida => partida.RodadaId).HasColumnName("rodada_id").HasMaxLength(20).IsRequired();
        builder.Property(partida => partida.SlotATipo).HasColumnName("slot_a_tipo").HasConversion<string>().HasMaxLength(20);
        builder.Property(partida => partida.SlotADuplaId).HasColumnName("slot_a_dupla_id");
        builder.Property(partida => partida.SlotASeed).HasColumnName("slot_a_seed");
        builder.Property(partida => partida.SlotAOrigem).HasColumnName("slot_a_origem");
        builder.Property(partida => partida.SlotBTipo).HasColumnName("slot_b_tipo").HasConversion<string>().HasMaxLength(20);
        builder.Property(partida => partida.SlotBDuplaId).HasColumnName("slot_b_dupla_id");
        builder.Property(partida => partida.SlotBSeed).HasColumnName("slot_b_seed");
        builder.Property(partida => partida.SlotBOrigem).HasColumnName("slot_b_origem");
        builder.Property(partida => partida.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
        builder.Property(partida => partida.AgendadaEm).HasColumnName("agendada_em");
        builder.Property(partida => partida.PlacarA).HasColumnName("placar_a");
        builder.Property(partida => partida.PlacarB).HasColumnName("placar_b");
        builder.Property(partida => partida.VencedorDuplaId).HasColumnName("vencedor_dupla_id");
        builder.HasIndex(partida => new { partida.TorneioId, partida.Numero }).IsUnique().HasFilter("[is_deleted] = 0");
        builder.HasOne<TorneioEntity>().WithMany().HasForeignKey(partida => partida.TorneioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Dupla>().WithMany().HasForeignKey(partida => partida.SlotADuplaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Dupla>().WithMany().HasForeignKey(partida => partida.SlotBDuplaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Dupla>().WithMany().HasForeignKey(partida => partida.VencedorDuplaId).OnDelete(DeleteBehavior.Restrict);
    }
}
