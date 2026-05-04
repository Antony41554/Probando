using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeejoshSystem.Domain.Entities;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class VentaConfiguration : IEntityTypeConfiguration<Venta>
    {
        public void Configure(EntityTypeBuilder<Venta> builder)
        {
            builder.ToTable("sale");

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(v => v.Fecha)
                .HasColumnName("date")
                .IsRequired();

            builder.Property(v => v.Total)
                .HasColumnName("total")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            // Punto de extensión para autenticación
            // builder.Property(v => v.UsuarioId).HasColumnName("user_id");

            // Backing field - EF Core usa _detalles, no la propiedad Detalles
            builder.Navigation(v => v.Detalles)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(v => v.Detalles)
                .WithOne()
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}