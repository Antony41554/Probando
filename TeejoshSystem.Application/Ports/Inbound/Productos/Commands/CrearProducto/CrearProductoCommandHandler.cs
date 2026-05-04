using MediatR;

using TeejoshSystem.Domain.Entities;
using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.ValueObjects;
using TeejoshSystem.Domain.Enums;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;
using TeejoshSystem.Application.Common;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Commands.CrearProducto
{
    public class CrearProductoCommandHandler : IRequestHandler<CrearProductoCommand, Result>
    {
        private readonly IProductoRepository _repository;

        public CrearProductoCommandHandler(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(
            CrearProductoCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Crear producto base
                var producto = new Producto(
                    request.Tipo,
                    new NombreProducto(request.Nombre),
                    new Precio(request.Precio),
                    new Unidades(request.Unidades)
                );

                // 2. Guardar producto para obtener ID
                var productoId = await _repository.AddAsync(producto);

                // 3. Crear y guardar detalle segun tipo
                await CrearYGuardarDetallePorTipo(productoId, producto, request);

                return Result.Success();
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                return Result.Failure("Error al crear el producto");
            }
        }

        private async Task CrearYGuardarDetallePorTipo(
            int productoId, 
            Producto producto,
            CrearProductoCommand request)
        {
            switch (request.Tipo)
            {
                case TipoProducto.HotWheels:
                    if (request.HotWheels is null)
                        throw new ArgumentException("Debe proporcionar detalles de Hot Wheels");

                    var hwDetalle = new HotWheelsDetalle(
                        request.HotWheels.Modelo,
                        request.HotWheels.Anio,
                        request.HotWheels.Serie,
                        request.HotWheels.CategoriaId
                    );

                    hwDetalle.AsignarProductoId(productoId);
                    producto.AsignarDescripcion(hwDetalle);
                    await _repository.AddHotWheelsDetalleAsync(hwDetalle);
                    break;

                case TipoProducto.Funko:
                    if (request.Funko is null)
                        throw new ArgumentException("Debe proporcionar detalles de Funko");

                    var funkoDetalle = new FunkoDetalle(
                        request.Funko.NumeroBox,
                        request.Funko.Licencia,
                        request.Funko.SubtipoId,
                        request.Funko.CaracteristicaEspecialId
                    );

                    funkoDetalle.AsignarProductoId(productoId);
                    producto.AsignarDescripcion(funkoDetalle);
                    await _repository.AddFunkoDetalleAsync(funkoDetalle);
                    break;

                case TipoProducto.Tcg:
                    if (request.Tcg is null)
                        throw new ArgumentException("Debe proporcionar detalles de TCG");

                    var tcgDetalle = new TcgDetalle(
                        request.Tcg.PackId,
                        request.Tcg.ExpansionId
                    );

                    tcgDetalle.AsignarProductoId(productoId);
                    producto.AsignarDescripcion(tcgDetalle);
                    await _repository.AddTcgDetalleAsync(tcgDetalle);
                    break;

                case TipoProducto.Toy:
                    if (request.Toy is null)
                        throw new ArgumentException("Debe proporcionar detalles de Toy");

                    var toyDetalle = new ToyDetalle(
                        request.Toy.EdadMinima,
                        request.Toy.JugadoresMinimo,
                        request.Toy.JugadoresMaximo,
                        request.Toy.EsJuegoMesa
                    );

                    toyDetalle.AsignarProductoId(productoId);
                    producto.AsignarDescripcion(toyDetalle);
                    await _repository.AddToyDetalleAsync(toyDetalle);
                    break;

                case TipoProducto.Varios:
                    if (request.Varios is null)
                        throw new ArgumentException("Debe proporcionar detalles de Varios");

                    var variosDetalle = new VariosDetalle(
                        request.Varios.Marca,
                        request.Varios.Alto,
                        request.Varios.Ancho,
                        request.Varios.Largo,
                        request.Varios.Material,
                        request.Varios.TieneIlustracion
                    );

                    variosDetalle.AsignarProductoId(productoId);
                    producto.AsignarDescripcion(variosDetalle);
                    await _repository.AddVariosDetalleAsync(variosDetalle);
                    break;

                default:
                    throw new ArgumentException("Tipo de producto no soportado");
            }
        }
    }
}
