using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class TcgFranquiciaConfiguration : IEntityTypeConfiguration<TcgFranquicia>
    {
        public void Configure(EntityTypeBuilder<TcgFranquicia> builder)
        {
            builder.ToTable("tcg_franchise");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Nombre)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(p => p.Nombre)
                .IsUnique();

            // Temporal
            builder.HasData(
                new TcgFranquicia { Id = 1, Nombre = "Pokémon" },
                new TcgFranquicia { Id = 2, Nombre = "Yu-Gi-Oh!" },
                new TcgFranquicia { Id = 3, Nombre = "Magic: The Gathering" },
                new TcgFranquicia { Id = 4, Nombre = "One Piece" },
                new TcgFranquicia { Id = 5, Nombre = "Bluey" }
            );
        }
    }
}