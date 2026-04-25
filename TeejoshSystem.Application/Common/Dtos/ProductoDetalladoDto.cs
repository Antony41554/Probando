using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Application.Common.Dtos
{
    public class ProductoDetalladoDto
    {
        public int Id { get; set; }
        public TipoProducto Tipo { get; set; }
        public required string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Unidades { get; set; }


        // Detalle especifico (polimorfico)
        public DetalleBaseDto? Detalle { get; set; }
    }

    public abstract class DetalleBaseDto
    {
        public TipoProducto Tipo { get; set; }
    }

    public class FunkoDetalleDto : DetalleBaseDto
    {
        public int NumeroBox { get; set; }
        public required string Licencia { get; set; }
        public int SubtipoId { get; set; }
        public required string SubtipoNombre { get; set; }
        public int? CaracteristicaEspecialId { get; set; }
        public string? CaracteristicaEspecialNombre { get; set; }
    }

    public class HotWheelsDetalleDto : DetalleBaseDto
    {
        public required string Modelo { get; set; }
        public int Anio { get; set; }
        public required string Serie { get; set; }
        public int CategoriaId { get; set; }
        public required string CategoriaNombre { get; set; }
    }

    public class TcgDetalleDto : DetalleBaseDto
    {
        public int PackId { get; set; }
        public required string PackNombre { get; set; }
        public int ExpansionId { get; set; }
        public required string ExpansionNombre { get; set; }
        public required string FranquiciaNombre { get; set; }
    }

    public class ToyDetalleDto : DetalleBaseDto
    {
        public int EdadMinima { get; set; }
        public int JugadoresMinimo { get; set; }
        public int JugadoresMaximo { get; set; }
        public bool EsJuegoMesa { get; set; }
    }

    public class VariosDetalleDto : DetalleBaseDto
    {
        public required string Marca { get; set; }
        public decimal Alto { get; set; }
        public decimal Ancho { get; set; }
        public decimal? Largo { get; set; }
        public required string Material { get; set; }
        public bool TieneIlustracion { get; set; }
    }
}
