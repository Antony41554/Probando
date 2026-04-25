using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class TcgConfiguration : IEntityTypeConfiguration<TcgDetalle>
    {
        public void Configure(EntityTypeBuilder<TcgDetalle> builder)
        {
            builder.HasBaseType((Type?)null);

            builder.ToTable("tcg");

            builder.HasKey(p => p.ProductoId);

            builder.Property(p => p.ProductoId)
                .HasColumnName("product_id");

            builder.Property(p => p.PackId)
                .HasColumnName("pack_id")
                .IsRequired();

            builder.Property(p => p.ExpansionId)
                .HasColumnName("expansion_id")
                .IsRequired();

            builder.HasOne<Producto>()
                .WithOne()
                .HasForeignKey<TcgDetalle>(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
