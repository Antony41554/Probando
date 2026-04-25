using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class TcgPackConfiguration : IEntityTypeConfiguration<TcgPack>
    {
        public void Configure(EntityTypeBuilder<TcgPack> builder)
        {
            builder.ToTable("tcg_pack");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Nombre)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => new { p.Nombre, p.FranquiciaId })
                .IsUnique();

            // Temporal
            builder.HasData(
                // Pokémon
                new TcgPack { Id = 1, Nombre = "Sobre Individual", FranquiciaId = 1 },
                new TcgPack { Id = 2, Nombre = "Blister 3 Sobres", FranquiciaId = 1 },
                new TcgPack { Id = 3, Nombre = "Elite Trainer Box", FranquiciaId = 1 },
                new TcgPack { Id = 4, Nombre = "Caja de 36 Sobres", FranquiciaId = 1 },
                new TcgPack { Id = 5, Nombre = "Colección Premium", FranquiciaId = 1 },
                // Yu-Gi-Oh!
                new TcgPack { Id = 6, Nombre = "Sobre Individual", FranquiciaId = 2 },
                new TcgPack { Id = 7, Nombre = "Caja de 24 Sobres", FranquiciaId = 2 },
                new TcgPack { Id = 8, Nombre = "Structure Deck", FranquiciaId = 2 },
                // Magic
                new TcgPack { Id = 9, Nombre = "Draft Booster", FranquiciaId = 3 },
                new TcgPack { Id = 10, Nombre = "Set Booster", FranquiciaId = 3 },
                new TcgPack { Id = 11, Nombre = "Collector Booster", FranquiciaId = 3 },
                new TcgPack { Id = 12, Nombre = "Bundle", FranquiciaId = 3 },
                // One Piece
                new TcgPack { Id = 13, Nombre = "Sobre Individual", FranquiciaId = 4 },
                new TcgPack { Id = 14, Nombre = "Caja de 24 Sobres", FranquiciaId = 4 },
                new TcgPack { Id = 15, Nombre = "Starter Deck", FranquiciaId = 4 },
                // Bluey
                new TcgPack { Id = 16, Nombre = "Sobre Individual", FranquiciaId = 5 },
                new TcgPack { Id = 17, Nombre = "Starter Pack", FranquiciaId = 5 }
            );
        }
    }
}