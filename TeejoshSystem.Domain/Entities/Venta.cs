using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.ValueObjects;

namespace TeejoshSystem.Domain.Entities
{
    public class Venta
    {
        public int Id { get; private set; }
        public DateTime Fecha { get; private set; }
        public decimal Total { get; private set; }

        // Punto de extensión para autenticación - activar cuando exista login
        // public int UsuarioId { get; private set; }

        private readonly List<VentaDetalle> _detalles = new();
        public IReadOnlyList<VentaDetalle> Detalles => _detalles.AsReadOnly();

        private Venta() { } // EF Core

        public Venta(DateTime fecha)
        {
            Fecha = fecha;
            Total = 0;
        }

        public void AgregarDetalle(VentaDetalle detalle)
        {
            ArgumentNullException.ThrowIfNull(detalle);
            _detalles.Add(detalle);
            Total += detalle.Subtotal;
        }
    }
}