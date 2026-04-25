using MediatR;

using TeejoshSystem.Domain.Ports.Outbound.Repositories;
using TeejoshSystem.Application.Common.Dtos;

namespace TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerCatalogos
{
    public class ObtenerCatalogosQueryHandler
        : IRequestHandler<ObtenerCatalogosQuery, CatalogosDto>
    {
        private readonly ICatalogoRepository _repository;

        public ObtenerCatalogosQueryHandler(ICatalogoRepository repository)
        {
            _repository = repository;
        }

        public async Task<CatalogosDto> Handle(
            ObtenerCatalogosQuery request,
            CancellationToken cancellationToken)
        {
            return new CatalogosDto
            {
                CategoriasHotWheels = (await _repository.GetHotWheelsCategoriasAsync())
                    .Select(c => new CatalogoItemDto { Id = c.Id, Nombre = c.Nombre }).ToList(),
                SubtiposFunko = (await _repository.GetFunkoSubtiposAsync())
                    .Select(s => new CatalogoItemDto { Id = s.Id, Nombre = s.Nombre }).ToList(),
                CaracteristicasFunko = (await _repository.GetFunkoCaracteristicasAsync())
                    .Select(c => new CatalogoItemDto { Id = c.Id, Nombre = c.Nombre }).ToList(),
                FranquiciasTcg = (await _repository.GetTcgFranquiciasAsync())
                    .Select(f => new CatalogoItemDto { Id = f.Id, Nombre = f.Nombre }).ToList()
            };
        }
    }
}