using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence
{
    public class InventarioDbContextFactory : IDesignTimeDbContextFactory<InventarioDbContext>
    {
        public InventarioDbContext CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TeejoshSystem",
                "inventario.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            var options = new DbContextOptionsBuilder<InventarioDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new InventarioDbContext(options);
        }
    }
}