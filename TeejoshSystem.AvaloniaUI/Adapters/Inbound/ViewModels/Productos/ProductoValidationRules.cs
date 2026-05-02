using System;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

public sealed class ProductoValidationRules
{
    public static ProductoValidationRules Default { get; } = new();

    public int NombreMaxLength { get; init; } = 50;
    public decimal PrecioMinimo { get; init; } = 0m;
    public string PrecioFormatoPattern { get; init; } = @"^\d+(\.\d{2})$";
    public string PrecioFormatoMensaje { get; init; } = "Formato de precio invalido (ejemplo valido: 152.79)";
    public int UnidadesMinimas { get; init; } = 0;

    public int TextoDetalleMaxLength { get; init; } = 50;
    public int HotWheelsAnioMinimo { get; init; } = 1967;
    public int HotWheelsAnioMaximo => DateTime.Now.Year + 1;

    public int FunkoNumeroCajaMinimo { get; init; } = 1;
    public int ToyJugadoresMinimo { get; init; } = 1;
    public decimal DimensionMinima { get; init; } = 0.01m;
}
