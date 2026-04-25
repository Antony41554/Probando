using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Configurations
{
    public class ToyConfiguration : IEntityTypeConfiguration<ToyDetalle>
    {
        public void Configure(EntityTypeBuilder<ToyDetalle> builder)
        {
            builder.HasBaseType((Type?)null);

            builder.ToTable("toy");

            builder.HasKey(p => p.ProductoId);

            builder.Property(p => p.ProductoId)
                .HasColumnName("product_id");

            builder.Property(p => p.EdadMinima)
                .HasColumnName("min_years_old")
                .IsRequired();

            builder.Property(p => p.JugadoresMin)
                .HasColumnName("min_players")
                .IsRequired();

            builder.Property(p => p.JugadoresMax)
                .HasColumnName("max_players")
                .IsRequired();

            builder.Property(p => p.EsJuegoDeMesa)
                .HasColumnName("is_board_game")
                .IsRequired();

            builder.ToTable("toy", p =>
            {
                p.HasCheckConstraint("check_players", "max_players >= min_players");
            });

            builder.HasOne<Producto>()
                .WithOne()
                .HasForeignKey<ToyDetalle>(p => p.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}