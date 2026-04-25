using MediatR;

using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Queries.BuscarProductos
{
    public record BuscarProductosQuery(
        string? Nombre,
        TipoProducto? Tipo
    ) : IRequest<List<ProductoBusquedaDto>>;
}
