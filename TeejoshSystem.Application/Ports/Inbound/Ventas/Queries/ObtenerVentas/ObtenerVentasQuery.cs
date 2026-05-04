using MediatR;
using TeejoshSystem.Application.Common.Dtos;

namespace TeejoshSystem.Application.Ports.Inbound.Ventas.Queries.ObtenerVentas
{
    public class ObtenerVentasQuery : IRequest<IReadOnlyList<VentaDto>>
    {
        public DateTime? Desde { get; }
        public DateTime? Hasta { get; }

        public ObtenerVentasQuery(DateTime? desde = null, DateTime? hasta = null)
        {
            Desde = desde;
            Hasta = hasta;
        }
    }
}