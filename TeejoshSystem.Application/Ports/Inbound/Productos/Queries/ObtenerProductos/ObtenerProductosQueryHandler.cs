using MediatR;

using TeejoshSystem.Application.Common.Dtos;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Queries.ObtenerProductos
{
    public class ObtenerProductosQueryHandler
            : IRequestHandler<ObtenerProductosQuery,
              IReadOnlyList<ProductoDto>>
    {
        private readonly IProductoRepository _productoRepository;

        public ObtenerProductosQueryHandler(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IReadOnlyList<ProductoDto>> Handle(
            ObtenerProductosQuery request,
            CancellationToken cancellationToken)
        {
            var productos = await _productoRepository.GetAllAsync();

            return productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Tipo = p.Tipo,
                Nombre = p.Nombre.Value,
                Precio = p.Precio.Value,
                Unidades = p.Stock.Value,
                TipoDescripcion = p.Tipo.ToString(),
                DetalleResumen = ""
            }).ToList();
        }
    }
}
