using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class VariosConfiguration : IEntityTypeConfiguration<VariosDetalle>
    {
        public void Configure(EntityTypeBuilder<VariosDetalle> builder)
        {
            builder.HasBaseType((Type?)null);

            builder.ToTable("varios");

            builder.HasKey(p => p.ProductoId);

            builder.Property(p => p.ProductoId)
                .HasColumnName("product_id");

            builder.Property(p => p.Marca)
                .HasColumnName("brand")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Alto)
                .HasColumnName("height")
                .IsRequired();

            builder.Property(p => p.Ancho)
                .HasColumnName("width")
                .IsRequired();

            builder.Property(p => p.Largo)
                .HasColumnName("length")
                .IsRequired();

            builder.Property(p => p.Material)
                .HasColumnName("material")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.TieneIlustracion)
                .HasColumnName("ilustration")
                .IsRequired();

            builder.ToTable("varios", p =>
            {
                p.HasCheckConstraint("check_dimensions",
                    "height > 0 AND width > 0 AND (length IS NULL OR length > 0)");
            });

            builder.HasOne<Producto>()
                .WithOne()
                .HasForeignKey<VariosDetalle>(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}