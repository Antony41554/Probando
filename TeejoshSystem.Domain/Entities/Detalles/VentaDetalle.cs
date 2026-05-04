

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public class VentaDetalle
    {
        public int Id { get; private set; }
        public int VentaId { get; private set; }
        public int ProductoId { get; private set; }
        public string NombreProducto { get; private set; } = null!;
        public int Cantidad { get; private set; }

        // Precio capturado en el momento de la venta - inmutable
        public decimal PrecioUnitario { get; private set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        private VentaDetalle() { } // EF Core

        public VentaDetalle(
            int productoId,
            string nombreProducto,
            int cantidad,
            decimal precioUnitario)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");
            if (precioUnitario < 0)
                throw new ArgumentException("El precio unitario no puede ser negativo.");
            if (string.IsNullOrWhiteSpace(nombreProducto))
                throw new ArgumentException("El nombre del producto es obligatorio.");

            ProductoId = productoId;
            NombreProducto = nombreProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }
    }
}