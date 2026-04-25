using TeejoshSystem.Application.Common.Dtos;

namespace TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerCatalogos
{
    public class CatalogosDto
    {
        public List<CatalogoItemDto> CategoriasHotWheels { get; set; } = new();
        public List<CatalogoItemDto> SubtiposFunko { get; set; } = new();
        public List<CatalogoItemDto> CaracteristicasFunko { get; set; } = new();
        public List<CatalogoItemDto> FranquiciasTcg { get; set; } = new();
    }
}
