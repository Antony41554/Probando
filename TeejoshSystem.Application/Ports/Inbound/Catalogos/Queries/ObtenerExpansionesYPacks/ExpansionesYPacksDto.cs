using TeejoshSystem.Application.Common.Dtos;

namespace TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerExpansionesYPacks
{
    public class ExpansionesYPacksDto
    {
        public List<CatalogoItemDto> Expansiones { get; set; } = new();
        public List<CatalogoItemDto> Packs { get; set; } = new();
    }
}
