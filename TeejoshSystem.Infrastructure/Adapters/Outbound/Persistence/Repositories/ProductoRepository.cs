using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.Enums;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Infrastructure.Adapters.Outbound.Persistence.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly InventarioDbContext _context;

    public ProductoRepository(InventarioDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Producto>> GetAllAsync()
    {
        return await _context.Productos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Producto?> GetByIdAsync(int id)
    {
        return await _context.Productos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<Producto>> SearchAsync(string? nombre, TipoProducto? tipo)
    {
        var query = _context.Productos.AsNoTracking();

        // Filtrar por nombre
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(p => p.Nombre.Value.Contains(nombre));
        }

        // Si hay filtro de tipo
        if (tipo.HasValue)
        {
            query = query.Where(p => p.Tipo == tipo.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<ProductoBusquedaResult>> SearchWithDetalleAsync(
    string? nombre, TipoProducto? tipo)
    {
        var sql = """
        SELECT 
            p.Id,
            p.type        AS Type,
            p.name        AS Name,
            p.price       AS Price,
            p.units       AS Units,
            CASE p.type
                WHEN 'HotWheels' THEN hw.model || ' · ' || CAST(hw.year AS TEXT) || ' · ' || hw.serie
                WHEN 'Funko'     THEN '#' || CAST(fu.box_number AS TEXT) || ' · ' || fu.license
                WHEN 'Tcg'       THEN 'Pack ' || CAST(tcg.pack_id AS TEXT) || ' · Expansión ' || CAST(tcg.expansion_id AS TEXT)
                WHEN 'Toy'       THEN CAST(toy.min_players AS TEXT) || '-' || CAST(toy.max_players AS TEXT) || ' jugadores'
                WHEN 'Varios'    THEN v.brand || ' · ' || v.material
                ELSE 'Sin detalle'
            END AS DetalleResumen
        FROM product p
        LEFT JOIN hot_wheels hw ON hw.product_id = p.Id
        LEFT JOIN funko fu      ON fu.product_id = p.Id
        LEFT JOIN tcg           ON tcg.product_id = p.Id
        LEFT JOIN toy           ON toy.product_id = p.Id
        LEFT JOIN varios v      ON v.product_id   = p.Id
        WHERE (@nombre IS NULL OR p.name LIKE '%' || @nombre || '%')
          AND (@tipo   IS NULL OR p.type = @tipo)
        """;

        var nombreParam = nombre is null or { Length: 0 }
            ? new SqliteParameter("@nombre", DBNull.Value)
            : new SqliteParameter("@nombre", nombre);

        var tipoParam = tipo.HasValue
            ? new SqliteParameter("@tipo", tipo.Value.ToString())
            : new SqliteParameter("@tipo", DBNull.Value);

        var resultados = await _context.Database
            .SqlQueryRaw<ProductoBusquedaRaw>(sql, nombreParam, tipoParam)
            .ToListAsync();

        // Debug temporal
        System.Diagnostics.Debug.WriteLine($"Resultados raw: {resultados.Count}");
        foreach (var r in resultados)
            System.Diagnostics.Debug.WriteLine($"  Id={r.Id} Type={r.Type} Name={r.Name}");

        return resultados.Select(r => new ProductoBusquedaResult(
            r.Id,
            Enum.Parse<TipoProducto>(r.Type),
            r.Name,
            r.Price,
            r.Units,
            r.DetalleResumen ?? "Sin detalle"
        )).ToList();
    }

    public async Task<Producto?> GetByIdWithDetalleAsync(int id)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto is null) return null;

        ProductoDetalle? detalle = producto.Tipo switch
        {
            TipoProducto.HotWheels => await _context.HotWheelsDetalles
                .FirstOrDefaultAsync(d => d.ProductoId == id),
            TipoProducto.Funko => await _context.FunkoDetalles
                .FirstOrDefaultAsync(d => d.ProductoId == id),
            TipoProducto.Tcg => await _context.TcgDetalles
                .FirstOrDefaultAsync(d => d.ProductoId == id),
            TipoProducto.Toy => await _context.ToyDetalles
                .FirstOrDefaultAsync(d => d.ProductoId == id),
            TipoProducto.Varios => await _context.VariosDetalles
                .FirstOrDefaultAsync(d => d.ProductoId == id),
            _ => null
        };

        if (detalle is not null)
            producto.AsignarDescripcion(detalle);

        return producto;
    }

    public async Task<int> AddAsync(Producto producto)
    {
        await _context.Productos.AddAsync(producto);
        await _context.SaveChangesAsync();
        return producto.Id;
    }

    public async Task UpdateAsync(Producto producto)
    {
        _context.Productos.Update(producto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Producto producto)
    {
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<int> productoIds)
    {
        var productos = await _context.Productos
            .Where(p => productoIds.Contains(p.Id))
            .ToListAsync();

        _context.Productos.RemoveRange(productos);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Productos
            .AnyAsync(p => p.Id == id);
    }

    // Metodos auxiliares para manejar detalles
    public async Task AddHotWheelsDetalleAsync(HotWheelsDetalle detalle)
    {
        await _context.HotWheelsDetalles.AddAsync(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task AddFunkoDetalleAsync(FunkoDetalle detalle)
    {
        await _context.FunkoDetalles.AddAsync(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task AddTcgDetalleAsync(TcgDetalle detalle)
    {
        await _context.TcgDetalles.AddAsync(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task AddToyDetalleAsync(ToyDetalle detalle)
    {
        await _context.ToyDetalles.AddAsync(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task AddVariosDetalleAsync(VariosDetalle detalle)
    {
        await _context.VariosDetalles.AddAsync(detalle);
        await _context.SaveChangesAsync();
    }

    // Métodos para ACTUALIZAR detalles
    public async Task UpdateHotWheelsDetalleAsync(HotWheelsDetalle detalle)
    {
        _context.HotWheelsDetalles.Update(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFunkoDetalleAsync(FunkoDetalle detalle)
    {
        _context.FunkoDetalles.Update(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTcgDetalleAsync(TcgDetalle detalle)
    {
        _context.TcgDetalles.Update(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateToyDetalleAsync(ToyDetalle detalle)
    {
        _context.ToyDetalles.Update(detalle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVariosDetalleAsync(VariosDetalle detalle)
    {
        _context.VariosDetalles.Update(detalle);
        await _context.SaveChangesAsync();
    }

    // Datatypes para la query SQL
    private sealed class ProductoBusquedaRaw
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Units { get; set; }
        public string? DetalleResumen { get; set; }
    }
}