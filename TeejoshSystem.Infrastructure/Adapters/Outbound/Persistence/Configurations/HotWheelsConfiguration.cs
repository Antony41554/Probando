using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class HotWheelsConfiguration : IEntityTypeConfiguration<HotWheelsDetalle>
    {
        public void Configure(EntityTypeBuilder<HotWheelsDetalle> builder)
        {
            builder.HasBaseType((Type?)null);

            builder.ToTable("hot_wheels");

            builder.HasKey(p => p.ProductoId);

            builder.Property(p => p.ProductoId)
                .HasColumnName("product_id");

            builder.Property(p => p.Modelo)
                .HasColumnName("model")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Anio)
                .HasColumnName("year")
                .IsRequired();

            builder.Property(p => p.Serie)
                .HasColumnName("serie")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.CategoriaId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.HasOne<Producto>()
                .WithOne()
                .HasForeignKey<HotWheelsDetalle>(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
