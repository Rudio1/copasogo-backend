using Copasogo.Domain.Jogadores.Entities;
using Copasogo.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Copasogo.Infra.Data.Jogadores.Mappings;

public sealed class JogadorConfiguration : IEntityTypeConfiguration<Jogador>
{
    public void Configure(EntityTypeBuilder<Jogador> builder)
    {
        builder.ToTable("jogadores");
        builder.ConfigureAuditColumns();
        builder.Property(jogador => jogador.Nome).HasColumnName("nome").HasMaxLength(80).IsRequired();
        builder.Property(jogador => jogador.SteamId).HasColumnName("steam_id").HasMaxLength(200).IsRequired();
        builder.HasIndex(jogador => jogador.SteamId).IsUnique().HasFilter("[is_deleted] = 0");
    }
}
