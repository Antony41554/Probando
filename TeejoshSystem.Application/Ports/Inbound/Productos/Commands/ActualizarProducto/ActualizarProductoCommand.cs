using MediatR;

using TeejoshSystem.Application.Common;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Commands.ActualizarProducto
{
    public record ActualizarProductoCommand(
        int Id,
        string Nombre,
        decimal Precio,
        int Unidades
    ) : IRequest<Result>;
}
