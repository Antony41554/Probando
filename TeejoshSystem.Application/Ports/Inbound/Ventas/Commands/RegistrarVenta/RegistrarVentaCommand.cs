using MediatR;
using TeejoshSystem.Application.Common;

namespace TeejoshSystem.Application.Ports.Inbound.Ventas.Commands.RegistrarVenta
{
    public class RegistrarVentaCommand : IRequest<Result<int>>
    {
        public List<RegistrarVentaItemCommand> Items { get; }

        public RegistrarVentaCommand(List<RegistrarVentaItemCommand> items)
        {
            if (items is null || items.Count == 0)
                throw new ArgumentException("La venta debe tener al menos un item.");
            Items = items;
        }
    }

    public class RegistrarVentaItemCommand
    {
        public int ProductoId { get; }
        public int Cantidad { get; }

        public RegistrarVentaItemCommand(int productoId, int cantidad)
        {
            ProductoId = productoId;
            Cantidad = cantidad;
        }
    }
}