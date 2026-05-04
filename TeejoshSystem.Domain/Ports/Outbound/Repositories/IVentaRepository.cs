using TeejoshSystem.Domain.Entities;

namespace TeejoshSystem.Domain.Ports.Outbound.Repositories
{
    public interface IVentaRepository
    {
        Task<int> AddAsync(Venta venta);
        Task<Venta?> GetByIdAsync(int id);
        Task<IReadOnlyList<Venta>> GetByFechaAsync(
            DateTime? desde,
            DateTime? hasta);
    }
}