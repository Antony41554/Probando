using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeejoshSystem.Domain.Entities.Detalles;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class VentaDetalleConfiguration : IEntityTypeConfiguration<VentaDetalle>
    {
        public void Configure(EntityTypeBuilder<VentaDetalle> builder)
        {
            builder.ToTable("sale_detail");

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.VentaId)
                .HasColumnName("sale_id")
                .IsRequired();

            builder.Property(d => d.ProductoId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.Property(d => d.NombreProducto)
                .HasColumnName("product_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.Cantidad)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(d => d.PrecioUnitario)
                .HasColumnName("unit_price")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            // Subtotal es calculado - no se persiste
            builder.Ignore(d => d.Subtotal);
        }
    }
}