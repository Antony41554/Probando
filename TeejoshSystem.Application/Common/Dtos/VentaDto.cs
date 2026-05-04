using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Application.Common.Dtos
{
    public class VentaDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<VentaDetalleDto> Detalles { get; set; } = new();
    }

    public class VentaDetalleDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = null!;
        public TipoProducto Tipo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}