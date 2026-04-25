using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class FunkoCaracteristicaConfiguration : IEntityTypeConfiguration<FunkoCaracteristica>
    {
        public void Configure(EntityTypeBuilder<FunkoCaracteristica> builder)
        {
            builder.ToTable("funko_special_feature");

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
                new FunkoCaracteristica { Id = 1, Nombre = "Chase" },
                new FunkoCaracteristica { Id = 2, Nombre = "Glow in the Dark" },
                new FunkoCaracteristica { Id = 3, Nombre = "Flocked" },
                new FunkoCaracteristica { Id = 4, Nombre = "Metallic" },
                new FunkoCaracteristica { Id = 5, Nombre = "Diamond / Glitter" },
                new FunkoCaracteristica { Id = 6, Nombre = "Black Light" },
                new FunkoCaracteristica { Id = 7, Nombre = "Chrome" },
                new FunkoCaracteristica { Id = 8, Nombre = "Translucent" },
                new FunkoCaracteristica { Id = 9, Nombre = "Exclusivo" }
            );
        }
    }
}