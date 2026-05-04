using MediatR;

using TeejoshSystem.Application.Common;
using TeejoshSystem.Application.Common.Dtos;
using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.Enums;
using TeejoshSystem.Domain.Ports.Outbound.Repositories;

namespace TeejoshSystem.Application.Ports.Inbound.Productos.Queries.ObtenerProductosPorId
{
    public class ObtenerProductosPorIdQueryHandler
        : IRequestHandler<ObtenerProductosPorIdQuery,
          Result<ProductoDetalladoDto>>
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICatalogoRepository _catalogoRepository;

        public ObtenerProductosPorIdQueryHandler(
            IProductoRepository productoRepository,
            ICatalogoRepository catalogoRepository)
        {
            _productoRepository = productoRepository;
            _catalogoRepository = catalogoRepository;
        }

        public async Task<Result<ProductoDetalladoDto>> Handle(
            ObtenerProductosPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var producto = await _productoRepository.GetByIdWithDetalleAsync(request.Id);

            if (producto is null)
                return Result.Failure<ProductoDetalladoDto>($"Producto con ID {request.Id} no encontrado.");

            var dto = new ProductoDetalladoDto
            {
                Id = producto.Id,
                Tipo = producto.Tipo,
                Nombre = producto.Nombre.Value,
                Precio = producto.Precio.Value,
                Unidades = producto.Stock.Value,
                Detalle = await MapearDetalleAsync(producto.Descripcion, producto.Tipo)
            };

            return Result.Success(dto);
        }

        private async Task<DetalleBaseDto?> MapearDetalleAsync(
            ProductoDetalle? descripcion,
            TipoProducto tipo)
        {
            switch (descripcion)
            {
                case HotWheelsDetalle hw:
                    {
                        // ⚠ Ajustar según los métodos reales de ICatalogoRepository
                        var categorias = await _catalogoRepository.GetHotWheelsCategoriasAsync();
                        var categoria = categorias.FirstOrDefault(c => c.Id == hw.CategoriaId);

                        return new HotWheelsDetalleDto
                        {
                            Tipo = tipo,
                            Modelo = hw.Modelo,
                            Anio = hw.Anio,
                            Serie = hw.Serie,
                            CategoriaId = hw.CategoriaId,
                            CategoriaNombre = categoria?.Nombre ?? $"Categoría {hw.CategoriaId}"
                        };
                    }

                case FunkoDetalle fu:
                    {
                        var subtipos = await _catalogoRepository.GetFunkoSubtiposAsync();
                        var subtipo = subtipos.FirstOrDefault(s => s.Id == fu.SubtipoId);

                        // CaracteristicaEspecial es nullable
                        string? caraNombre = null;
                        if (fu.CaracteristicaEspecialId.HasValue)
                        {
                            var caras = await _catalogoRepository.GetFunkoCaracteristicasAsync();
                            caraNombre = caras.FirstOrDefault(c => c.Id == fu.CaracteristicaEspecialId)?.Nombre;
                        }

                        return new FunkoDetalleDto
                        {
                            Tipo = tipo,
                            NumeroBox = fu.NumeroCaja,
                            Licencia = fu.Licencia,
                            SubtipoId = fu.SubtipoId,
                            SubtipoNombre = subtipo?.Nombre ?? $"Subtipo {fu.SubtipoId}",
                            CaracteristicaEspecialId = fu.CaracteristicaEspecialId,
                            CaracteristicaEspecialNombre = caraNombre
                        };
                    }

                case TcgDetalle tcg:
                    {
                        var expansion = await _catalogoRepository.GetTcgExpansionByIdAsync(tcg.ExpansionId);
                        var pack = await _catalogoRepository.GetTcgPackByIdAsync(tcg.PackId);

                        // Franquicia via FranquiciaId que ya tiene la expansión
                        var franquicias = await _catalogoRepository.GetTcgFranquiciasAsync();
                        var franquicia = expansion is not null
                            ? franquicias.FirstOrDefault(f => f.Id == expansion.FranquiciaId)
                            : null;

                        return new TcgDetalleDto
                        {
                            Tipo = tipo,
                            PackId = tcg.PackId,
                            PackNombre = pack?.Nombre ?? $"Pack {tcg.PackId}",
                            ExpansionId = tcg.ExpansionId,
                            ExpansionNombre = expansion?.Nombre ?? $"Expansión {tcg.ExpansionId}",
                            FranquiciaNombre = franquicia?.Nombre ?? "Sin franquicia"
                        };
                    }

                case ToyDetalle toy:
                    return new ToyDetalleDto
                    {
                        Tipo = tipo,
                        EdadMinima = toy.EdadMinima,
                        JugadoresMinimo = toy.JugadoresMin,
                        JugadoresMaximo = toy.JugadoresMax,
                        EsJuegoMesa = toy.EsJuegoDeMesa
                    };

                case VariosDetalle va:
                    return new VariosDetalleDto
                    {
                        Tipo = tipo,
                        Marca = va.Marca,
                        Alto = va.Alto,
                        Ancho = va.Ancho,
                        Largo = va.Largo,
                        Material = va.Material,
                        TieneIlustracion = va.TieneIlustracion
                    };

                default:
                    return null;
            }
        }
    }
}
