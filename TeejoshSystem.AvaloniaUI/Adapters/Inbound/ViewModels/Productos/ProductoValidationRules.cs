using System;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

public sealed class ProductoValidationRules
{
    public static ProductoValidationRules Default { get; } = new();

    public int NombreMaxLength { get; init; } = 100;
    public decimal PrecioMinimo { get; init; } = 0m;
    public int UnidadesMinimas { get; init; } = 0;

    public int TextoDetalleMaxLength { get; init; } = 100;
    public int HotWheelsAnioMinimo { get; init; } = 1967;
    public int HotWheelsAnioMaximo => DateTime.Now.Year + 1;

    public int FunkoNumeroCajaMinimo { get; init; } = 1;
    public int ToyJugadoresMinimo { get; init; } = 1;
    public decimal DimensionMinima { get; init; } = 0.01m;
}
