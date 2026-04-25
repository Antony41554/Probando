using MediatR;

namespace TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerCatalogos
{
    public record ObtenerCatalogosQuery : IRequest<CatalogosDto>;
}