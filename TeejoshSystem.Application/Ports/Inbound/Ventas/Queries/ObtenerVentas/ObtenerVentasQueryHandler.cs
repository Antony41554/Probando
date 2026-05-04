using MediatR;
using TeejoshSystem.Application.Common.Dtos;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Application.Ports.Inbound.Ventas.Queries.ObtenerVentas
{
    public class ObtenerVentasQueryHandler
        : IRequestHandler<ObtenerVentasQuery, IReadOnlyList<VentaDto>>
    {
        private readonly IVentaRepository _ventaRepository;

        public ObtenerVentasQueryHandler(IVentaRepository ventaRepository)
        {
            _ventaRepository = ventaRepository;
        }

        public async Task<IReadOnlyList<VentaDto>> Handle(
            ObtenerVentasQuery request,
            CancellationToken cancellationToken)
        {
            var ventas = await _ventaRepository.GetByFechaAsync(
                request.Desde,
                request.Hasta);

            // Orden descendente por fecha - más reciente primero
            return ventas
                .OrderByDescending(v => v.Fecha)
                .Select(v => new VentaDto
                {
                    Id = v.Id,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    Detalles = v.Detalles.Select(d => new VentaDetalleDto
                    {
                        ProductoId = d.ProductoId,
                        NombreProducto = d.NombreProducto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                }).ToList();
        }
    }
}