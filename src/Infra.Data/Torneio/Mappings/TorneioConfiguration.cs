using Copasogo.Domain.Torneio;
using Copasogo.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneioEntity = Copasogo.Domain.Torneio.Entities.Torneio;

namespace Copasogo.Infra.Data.Torneio.Mappings;

public sealed class TorneioConfiguration : IEntityTypeConfiguration<TorneioEntity>
{
    public void Configure(EntityTypeBuilder<TorneioEntity> builder)
    {
        builder.ToTable("torneios");
        builder.ConfigureAuditColumns();
        builder.Property(torneio => torneio.Nome).HasColumnName("nome").HasMaxLength(120).IsRequired();
        builder.Property(torneio => torneio.Slug).HasColumnName("slug").HasMaxLength(80).IsRequired();
        builder.Property(torneio => torneio.Edicao).HasColumnName("edicao");
        builder.Property(torneio => torneio.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
        builder.Property(torneio => torneio.EntryFeeCents).HasColumnName("entry_fee_cents");
        builder.Property(torneio => torneio.PrizePoolCents).HasColumnName("prize_pool_cents");
        builder.Property(torneio => torneio.QuantidadePartidas).HasColumnName("quantidade_partidas");
        builder.Property(torneio => torneio.IniciadoEm).HasColumnName("iniciado_em");
        builder.Property(torneio => torneio.EncerradoEm).HasColumnName("encerrado_em");
        builder.HasIndex(torneio => torneio.Slug).IsUnique().HasFilter("[is_deleted] = 0");
        builder.HasIndex(torneio => torneio.Edicao).IsUnique().HasFilter("[is_deleted] = 0");
    }
}
