using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Domain.Ports.Outbound.Repositories
{
    public interface IProductoRepository
    {
        // Consultas
        Task<IReadOnlyList<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<IReadOnlyList<Producto>> SearchAsync(string? nombre, TipoProducto? tipo);

        // Obtener con detalles especificos
        Task<Producto?> GetByIdWithDetalleAsync(int id);
        Task<IReadOnlyList<ProductoBusquedaResult>> SearchWithDetalleAsync(
            string? nombre, TipoProducto? tipo);

        // Comandos - Producto
        Task<int> AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(Producto producto);
        Task DeleteRangeAsync(IEnumerable<int> productoIds);

        // Comandos - Detalles (Crear)
        Task AddHotWheelsDetalleAsync(HotWheelsDetalle detalle);
        Task AddFunkoDetalleAsync(FunkoDetalle detalle);
        Task AddTcgDetalleAsync(TcgDetalle detalle);
        Task AddToyDetalleAsync(ToyDetalle detalle);
        Task AddVariosDetalleAsync(VariosDetalle detalle);

        // Comandos - Detalles (Actualizar)
        Task UpdateHotWheelsDetalleAsync(HotWheelsDetalle detalle);
        Task UpdateFunkoDetalleAsync(FunkoDetalle detalle);
        Task UpdateTcgDetalleAsync(TcgDetalle detalle);
        Task UpdateToyDetalleAsync(ToyDetalle detalle);
        Task UpdateVariosDetalleAsync(VariosDetalle detalle);

        // Utilidades
        Task<bool> ExistsAsync(int id);
    }
}
