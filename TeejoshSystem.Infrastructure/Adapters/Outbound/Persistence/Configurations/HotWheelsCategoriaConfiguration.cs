using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class HotWheelsCategoriaConfiguration : IEntityTypeConfiguration<HotWheelsCategoria>
    {
        public void Configure(EntityTypeBuilder<HotWheelsCategoria> builder)
        {
            builder.ToTable("hot_wheels_category");

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
                new HotWheelsCategoria { Id = 1, Nombre = "Basic Car" },
                new HotWheelsCategoria { Id = 2, Nombre = "Treasure Hunt" },
                new HotWheelsCategoria { Id = 3, Nombre = "Super Treasure Hunt" },
                new HotWheelsCategoria { Id = 4, Nombre = "Car Culture" },
                new HotWheelsCategoria { Id = 5, Nombre = "Premium" },
                new HotWheelsCategoria { Id = 6, Nombre = "Boulevard" },
                new HotWheelsCategoria { Id = 7, Nombre = "Pop Culture" },
                new HotWheelsCategoria { Id = 8, Nombre = "Team Transport" },
                new HotWheelsCategoria { Id = 9, Nombre = "Mystery Models" },
                new HotWheelsCategoria { Id = 10, Nombre = "HWC / RLC" }
            );
        }
    }
}
