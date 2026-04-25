using MediatR;

using TeejoshSystem.Application.Common.Dtos;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerExpansionesYPacks
{
    public class ObtenerExpansionesYPacksQueryHandler
        : IRequestHandler<ObtenerExpansionesYPacksQuery, ExpansionesYPacksDto>
    {
        private readonly ICatalogoRepository _repository;

        public ObtenerExpansionesYPacksQueryHandler(ICatalogoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExpansionesYPacksDto> Handle(
            ObtenerExpansionesYPacksQuery request,
            CancellationToken cancellationToken)
        {
            return new ExpansionesYPacksDto
            {
                Expansiones = (await _repository.GetTcgExpansionesAsync(request.FranquiciaId))
                    .Select(e => new CatalogoItemDto { Id = e.Id, Nombre = e.Nombre }).ToList(),
                Packs = (await _repository.GetTcgPacksAsync(request.FranquiciaId))
                    .Select(p => new CatalogoItemDto { Id = p.Id, Nombre = p.Nombre }).ToList()
            };
        }
    }
}