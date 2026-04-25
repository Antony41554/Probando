using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Queries.BuscarProductos
{
    public class ProductoBusquedaDto
    {
        public int Id { get; set; }
        public TipoProducto Tipo { get; set; }
        public required string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Unidades { get; set; }
        public required string DetalleResumen { get; set; }
    }
}
