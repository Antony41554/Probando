using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("product");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Tipo)
                .HasColumnName("type")
                .HasConversion<string>()  // guarda como texto, más legible
                .IsRequired();

            // Value Object: Nombre
            builder.OwnsOne(p => p.Nombre, p =>
            {
                p.Property(x => x.Value)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // Value Object: Precio
            builder.OwnsOne(p => p.Precio, p =>
            {
                p.Property(x => x.Value)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();
            });

            // Value Object: Stock
            builder.OwnsOne(p => p.Stock, p =>
            {
                p.Property(x => x.Value)
                    .HasColumnName("units")
                    .IsRequired();
            });

            // Descripcion es una navigation property resuelta en memoria por el repositorio.
            // EF Core no la gestiona porque ProductoDetalle es abstracta (TPT sin tabla base).
            // El repositorio carga el detalle por separado usando Producto.Tipo.

            builder.Ignore(p => p.Descripcion);
        }
    }
}
