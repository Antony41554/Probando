using Microsoft.EntityFrameworkCore;

using TeejoshSystem.Domain.Entities.Catalogos;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Repositories
{
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly InventarioDbContext _context;

        public CatalogoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        // Hot Wheels
        public async Task<List<HotWheelsCategoria>> GetHotWheelsCategoriasAsync()
        {
            return await _context.Set<HotWheelsCategoria>()
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        // Funko
        public async Task<List<FunkoSubtipo>> GetFunkoSubtiposAsync()
        {
            return await _context.Set<FunkoSubtipo>()
                .AsNoTracking()
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<List<FunkoCaracteristica>> GetFunkoCaracteristicasAsync()
        {
            return await _context.Set<FunkoCaracteristica>()
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        // TCG
        public async Task<List<TcgFranquicia>> GetTcgFranquiciasAsync()
        {
            return await _context.Set<TcgFranquicia>()
                .AsNoTracking()
                .OrderBy(f => f.Nombre)
                .ToListAsync();
        }

        public async Task<List<TcgExpansion>> GetTcgExpansionesAsync(int franquiciaId)
        {
            return await _context.Set<TcgExpansion>()
                .AsNoTracking()
                .Where(e => e.FranquiciaId == franquiciaId)
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<List<TcgPack>> GetTcgPacksAsync(int franquiciaId)
        {
            return await _context.Set<TcgPack>()
                .AsNoTracking()
                .Where(p => p.FranquiciaId == franquiciaId)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
    }
}
