using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class FunkoConfiguration : IEntityTypeConfiguration<FunkoDetalle>
    {
        public void Configure(EntityTypeBuilder<FunkoDetalle> builder)
        {
            builder.HasBaseType((Type?)null);

            builder.ToTable("funko");

            builder.HasKey(p => p.ProductoId);

            builder.Property(p => p.ProductoId)
                .HasColumnName("product_id");

            builder.Property(p => p.NumeroCaja)
                .HasColumnName("box_number")
                .IsRequired();

            builder.Property(p => p.Licencia)
                .HasColumnName("license")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.SubtipoId)
                .HasColumnName("subtype")
                .IsRequired();

            builder.Property(p => p.CaracteristicaEspecialId)
                .HasColumnName("special_caracteristic")
                .IsRequired();

            builder.HasOne<Producto>()
                .WithOne()
                .HasForeignKey<FunkoDetalle>(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
