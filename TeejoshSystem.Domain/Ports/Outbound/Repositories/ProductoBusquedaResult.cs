using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.Domain.Ports.Outbound.Repositories;

public record ProductoBusquedaResult(
    int Id,
    TipoProducto Tipo,
    string Nombre,
    decimal Precio,
    int Unidades,
    string DetalleResumen
);