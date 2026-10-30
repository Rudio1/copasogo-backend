using Copasogo.Domain.Duplas.Entities;
using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Infra.Data.Duplas.Mappings;

public sealed class DuplaConfiguration : IEntityTypeConfiguration<Dupla>
{
    public void Configure(EntityTypeBuilder<Dupla> builder)
    {
        builder.ToTable("duplas");
        builder.ConfigureAuditColumns();
        builder.Property(dupla => dupla.TorneioId).HasColumnName("torneio_id");
        builder.Property(dupla => dupla.Nome).HasColumnName("nome").HasMaxLength(80).IsRequired();
        builder.Property(dupla => dupla.Seed).HasColumnName("seed").IsRequired(false);
        builder.Property(dupla => dupla.JogadorAId).HasColumnName("jogador_a_id");
        builder.Property(dupla => dupla.JogadorBId).HasColumnName("jogador_b_id");
        builder.HasIndex(dupla => new { dupla.TorneioId, dupla.Seed }).IsUnique().HasFilter("[is_deleted] = 0 AND [seed] IS NOT NULL");
        builder.HasOne<TorneioEntity>().WithMany().HasForeignKey(dupla => dupla.TorneioId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Jogador>().WithMany().HasForeignKey(dupla => dupla.JogadorAId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Jogador>().WithMany().HasForeignKey(dupla => dupla.JogadorBId).OnDelete(DeleteBehavior.Restrict);
    }
}
