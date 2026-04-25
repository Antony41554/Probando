using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class FunkoSubtipoConfiguration : IEntityTypeConfiguration<FunkoSubtipo>
    {
        public void Configure(EntityTypeBuilder<FunkoSubtipo> builder)
        {
            builder.ToTable("funko_subtype");

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
                new FunkoSubtipo { Id = 1, Nombre = "Pop! Vinyl" },
                new FunkoSubtipo { Id = 2, Nombre = "Pop! Deluxe" },
                new FunkoSubtipo { Id = 3, Nombre = "Pop! Super" },
                new FunkoSubtipo { Id = 4, Nombre = "Pop! Mega" },
                new FunkoSubtipo { Id = 5, Nombre = "Pop! Rides" },
                new FunkoSubtipo { Id = 6, Nombre = "Pop! Moments" },
                new FunkoSubtipo { Id = 7, Nombre = "Pop! Albums" },
                new FunkoSubtipo { Id = 8, Nombre = "Bitty Pop!" },
                new FunkoSubtipo { Id = 9, Nombre = "Funko Soda" },
                new FunkoSubtipo { Id = 10, Nombre = "Mystery Minis" }
            );
        }
    }
}