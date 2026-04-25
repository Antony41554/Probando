using MediatR;

using TeejoshSystem.Domain.ValueObjects;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;
using TeejoshSystem.Application.Common;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Commands.ActualizarProducto
{
    public class ActualizarProductoCommandHandler : IRequestHandler<ActualizarProductoCommand, Result>
    {
        private readonly IProductoRepository _repository;

        public ActualizarProductoCommandHandler(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(
            ActualizarProductoCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var producto = await _repository.GetByIdAsync(request.Id);

                if (producto is null)
                    return Result.Failure("El producto no existe.");
                
                producto.ActualizarDatos(
                    new NombreProducto(request.Nombre),
                    new Precio(request.Precio),
                    new Unidades(request.Unidades));

                await _repository.UpdateAsync(producto);

                return Result.Success();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                return Result.Failure("No se pudo guardar el producto. Intente nuevamente.");
            }
        }
    }
}
