using MediatR;

using TeejoshSystem.Application.Common.Dtos;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Queries.ObtenerProductos
{
    public class ObtenerProductosQuery : IRequest<IReadOnlyList<ProductoDto>> { }
}
