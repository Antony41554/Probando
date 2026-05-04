using Microsoft.EntityFrameworkCore;
using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly InventarioDbContext _context;

        public VentaRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Venta venta)
        {
            await _context.Ventas.AddAsync(venta);
            await _context.SaveChangesAsync();
            return venta.Id;
        }

        public async Task<Venta?> GetByIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IReadOnlyList<Venta>> GetByFechaAsync(
            DateTime? desde,
            DateTime? hasta)
        {
            var query = _context.Ventas
                .Include(v => v.Detalles)
                .AsNoTracking();

            if (desde.HasValue)
                query = query.Where(v => v.Fecha >= desde.Value);

            if (hasta.HasValue)
                // Hasta fin del día seleccionado
                query = query.Where(v => v.Fecha <= hasta.Value.Date.AddDays(1).AddTicks(-1));

            return await query.ToListAsync();
        }
    }
}