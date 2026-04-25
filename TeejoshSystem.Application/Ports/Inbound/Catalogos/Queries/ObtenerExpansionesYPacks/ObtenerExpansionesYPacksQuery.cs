using MediatR;

namespace TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerExpansionesYPacks
{
    public record ObtenerExpansionesYPacksQuery(int FranquiciaId)
        : IRequest<ExpansionesYPacksDto>;
}