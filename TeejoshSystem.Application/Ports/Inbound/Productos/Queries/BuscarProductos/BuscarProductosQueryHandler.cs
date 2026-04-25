using MediatR;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Queries.BuscarProductos
{
    public class BuscarProductosQueryHandler : IRequestHandler<BuscarProductosQuery, List<ProductoBusquedaDto>>
    {
        private readonly IProductoRepository _repository;

        public BuscarProductosQueryHandler(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductoBusquedaDto>> Handle(
            BuscarProductosQuery request,
            CancellationToken cancellationToken)
        {
            var resultados = await _repository.SearchWithDetalleAsync(
                request.Nombre, request.Tipo);

            return resultados.Select(p => new ProductoBusquedaDto
            {
                Id = p.Id,
                Tipo = p.Tipo,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Unidades = p.Unidades,
                DetalleResumen = p.DetalleResumen
            }).ToList();
        }
    }
}
