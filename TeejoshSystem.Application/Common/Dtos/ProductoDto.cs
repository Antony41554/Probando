using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Application.Common.Dtos
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public TipoProducto Tipo { get; set; }
        public required string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Unidades { get; set; }

        public required string TipoDescripcion { get; set; }

        // Resumen del detalle (para mostrar en lista)
        public required string DetalleResumen { get; set; }
    }
}
