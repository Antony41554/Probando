using MediatR;

using TeejoshSystem.Application.Common;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Commands.EliminarProducto
{
    public record EliminarProductosCommand(List<int> ProductoIds) : IRequest<Result>;
}
