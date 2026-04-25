using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class TcgExpansionConfiguration : IEntityTypeConfiguration<TcgExpansion>
    {
        public void Configure(EntityTypeBuilder<TcgExpansion> builder)
        {
            builder.ToTable("tcg_expansion");

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
                new TcgExpansion { Id = 1, Nombre = "Escarlata y Púrpura Base", FranquiciaId = 1 },
                new TcgExpansion { Id = 2, Nombre = "151", FranquiciaId = 1 },
                new TcgExpansion { Id = 3, Nombre = "Obsidiana Llameante", FranquiciaId = 1 },
                new TcgExpansion { Id = 4, Nombre = "Destinos de Paldea", FranquiciaId = 1 },
                new TcgExpansion { Id = 5, Nombre = "Fuerza Temporal", FranquiciaId = 1 },
                // Yu-Gi-Oh!
                new TcgExpansion { Id = 6, Nombre = "Legendary Collection", FranquiciaId = 2 },
                new TcgExpansion { Id = 7, Nombre = "Age of Overlord", FranquiciaId = 2 },
                new TcgExpansion { Id = 8, Nombre = "Phantom Nightmare", FranquiciaId = 2 },
                // Magic: The Gathering
                new TcgExpansion { Id = 9, Nombre = "Wilds of Eldraine", FranquiciaId = 3 },
                new TcgExpansion { Id = 10, Nombre = "The Lost Caverns of Ixalan", FranquiciaId = 3 },
                new TcgExpansion { Id = 11, Nombre = "Murders at Karlov Manor", FranquiciaId = 3 },
                // One Piece
                new TcgExpansion { Id = 12, Nombre = "Romance Dawn", FranquiciaId = 4 },
                new TcgExpansion { Id = 13, Nombre = "Paramount War", FranquiciaId = 4 },
                new TcgExpansion { Id = 14, Nombre = "Pillars of Strength", FranquiciaId = 4 },
                new TcgExpansion { Id = 15, Nombre = "Kingdoms of Intrigue", FranquiciaId = 4 },
                // Bluey
                new TcgExpansion { Id = 16, Nombre = "Serie Base", FranquiciaId = 5 }
            );
        }
    }
}