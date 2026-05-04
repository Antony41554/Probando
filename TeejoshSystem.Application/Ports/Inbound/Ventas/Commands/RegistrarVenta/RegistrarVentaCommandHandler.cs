using MediatR;
using TeejoshSystem.Application.Common;
using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Application.Ports.Inbound.Ventas.Commands.RegistrarVenta
{
    public class RegistrarVentaCommandHandler
        : IRequestHandler<RegistrarVentaCommand, Result<int>>
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IProductoRepository _productoRepository;

        public RegistrarVentaCommandHandler(
            IVentaRepository ventaRepository,
            IProductoRepository productoRepository)
        {
            _ventaRepository = ventaRepository;
            _productoRepository = productoRepository;
        }

        public async Task<Result<int>> Handle(
            RegistrarVentaCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Cargar todos los productos involucrados
                var productos = new List<Producto>();
                foreach (var item in request.Items)
                {
                    var producto = await _productoRepository.GetByIdAsync(item.ProductoId);
                    if (producto is null)
                        return Result.Failure<int>(
                            $"Producto con ID {item.ProductoId} no encontrado.");

                    if (producto.Stock.Value < item.Cantidad)
                        return Result.Failure<int>(
                            $"Stock insuficiente para '{producto.Nombre.Value}'. " +
                            $"Disponible: {producto.Stock.Value}, solicitado: {item.Cantidad}.");

                    productos.Add(producto);
                }

                // 2. Crear la venta
                var venta = new Venta(DateTime.Now);

                foreach (var item in request.Items)
                {
                    var producto = productos.First(p => p.Id == item.ProductoId);

                    var detalle = new VentaDetalle(
                        producto.Id,
                        producto.Nombre.Value,
                        item.Cantidad,
                        producto.Precio.Value);

                    venta.AgregarDetalle(detalle);
                }

                // 3. Persistir la venta (atómico - dentro del mismo DbContext)
                var ventaId = await _ventaRepository.AddAsync(venta);

                // 4. Decrementar stock de cada producto
                foreach (var item in request.Items)
                {
                    var producto = productos.First(p => p.Id == item.ProductoId);
                    producto.ReducirStock(item.Cantidad);
                    await _productoRepository.UpdateAsync(producto);
                }

                return Result.Success(ventaId);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<int>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure<int>(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Result.Failure<int>("Error al registrar la venta. Intente nuevamente.");
            }
        }
    }
}