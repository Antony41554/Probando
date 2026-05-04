using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TeejoshSystem.Application.Common.Dtos;
using TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerCatalogos;
using TeejoshSystem.Application.Ports.Inbound.Catalogos.Queries.ObtenerExpansionesYPacks;
using TeejoshSystem.Application.Ports.Inbound.Productos.Commands.CrearProducto;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;
using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

public partial class CrearProductoViewModel : ValidatableViewModel
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly IConfirmationService _confirmation;
    private readonly INavigationService _navigation;
    private readonly ProductoValidationRules _validationRules = ProductoValidationRules.Default;

    [ObservableProperty] private string? _nombre;
    [ObservableProperty] private string? _precioTexto = "0.00";
    [ObservableProperty] private string? _unidadesTexto = "0";
    [ObservableProperty] private TipoProductoFiltroItem? _tipoSeleccionado;
    [ObservableProperty] private bool _catalogosCargados;

    public FieldValidationState NombreValidation { get; }
    public FieldValidationState PrecioValidation { get; }
    public FieldValidationState UnidadesValidation { get; }
    public FieldValidationState HwModeloValidation { get; }
    public FieldValidationState HwAnioValidation { get; }
    public FieldValidationState HwSerieValidation { get; }
    public FieldValidationState FunkoNumeroBoxValidation { get; }
    public FieldValidationState FunkoLicenciaValidation { get; }
    public FieldValidationState ToyJugadoresMinValidation { get; }
    public FieldValidationState ToyJugadoresMaxValidation { get; }
    public FieldValidationState ToyEdadMinimaValidation { get; }
    public FieldValidationState VariosMarcaValidation { get; }
    public FieldValidationState VariosAltoValidation { get; }
    public FieldValidationState VariosAnchoValidation { get; }
    public FieldValidationState VariosLargoValidation { get; }
    public FieldValidationState VariosMaterialValidation { get; }

    [ObservableProperty] private string? _hwModelo;
    [ObservableProperty] private string? _hwAnioTexto = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
    [ObservableProperty] private string? _hwSerie;
    [ObservableProperty] private CatalogoItemDto? _hwCategoriaSeleccionada;

    public ObservableCollection<CatalogoItemDto> CategoriasHotWheels { get; } = new();

    [ObservableProperty] private string? _funkoNumeroBoxTexto = "0";
    [ObservableProperty] private string? _funkoLicencia;
    [ObservableProperty] private CatalogoItemDto? _funkoSubtipoSeleccionado;
    [ObservableProperty] private CatalogoItemDto? _funkoCaracteristicaSeleccionada;

    public ObservableCollection<CatalogoItemDto> SubtiposFunko { get; } = new();
    public ObservableCollection<CatalogoItemDto> CaracteristicasFunko { get; } = new();

    [ObservableProperty] private CatalogoItemDto? _tcgFranquiciaSeleccionada;
    [ObservableProperty] private CatalogoItemDto? _tcgExpansionSeleccionada;
    [ObservableProperty] private CatalogoItemDto? _tcgPackSeleccionado;

    public ObservableCollection<CatalogoItemDto> FranquiciasTcg { get; } = new();
    public ObservableCollection<CatalogoItemDto> TcgExpansionesDisponibles { get; } = new();
    public ObservableCollection<CatalogoItemDto> TcgPacksDisponibles { get; } = new();

    [ObservableProperty] private string? _toyEdadMinimaTexto = "0";
    [ObservableProperty] private string? _toyJugadoresMinTexto = "1";
    [ObservableProperty] private string? _toyJugadoresMaxTexto = "1";
    [ObservableProperty] private bool _toyEsJuegoMesa;

    [ObservableProperty] private string? _variosMarca;
    [ObservableProperty] private string? _variosAltoTexto = "0";
    [ObservableProperty] private string? _variosAnchoTexto = "0";
    [ObservableProperty] private string? _variosLargoTexto;
    [ObservableProperty] private string? _variosMaterial;
    [ObservableProperty] private bool _variosTieneIlustracion;

    public bool MostrarHotWheels => TipoSeleccionado?.Valor == TipoProducto.HotWheels;
    public bool MostrarFunko => TipoSeleccionado?.Valor == TipoProducto.Funko;
    public bool MostrarTcg => TipoSeleccionado?.Valor == TipoProducto.Tcg;
    public bool MostrarToy => TipoSeleccionado?.Valor == TipoProducto.Toy;
    public bool MostrarVarios => TipoSeleccionado?.Valor == TipoProducto.Varios;

    public ObservableCollection<TipoProductoFiltroItem> TiposDisponibles { get; } = new()
    {
        new TipoProductoFiltroItem("Hot Wheels", TipoProducto.HotWheels),
        new TipoProductoFiltroItem("Funko", TipoProducto.Funko),
        new TipoProductoFiltroItem("TCG", TipoProducto.Tcg),
        new TipoProductoFiltroItem("Toy", TipoProducto.Toy),
        new TipoProductoFiltroItem("Varios", TipoProducto.Varios)
    };

    public CrearProductoViewModel(
        IMediator mediator,
        INotificationService notification,
        IConfirmationService confirmation,
        INavigationService navigation)
    {
        _mediator = mediator;
        _notification = notification;
        _confirmation = confirmation;
        _navigation = navigation;

        NombreValidation = RegisterFieldValidation(nameof(Nombre), ValidateNombre);
        PrecioValidation = RegisterFieldValidation(nameof(PrecioTexto), ValidatePrecio);
        UnidadesValidation = RegisterFieldValidation(nameof(UnidadesTexto), ValidateUnidades);
        HwModeloValidation = RegisterFieldValidation(nameof(HwModelo), ValidateHwModelo);
        HwAnioValidation = RegisterFieldValidation(nameof(HwAnioTexto), ValidateHwAnio);
        HwSerieValidation = RegisterFieldValidation(nameof(HwSerie), ValidateHwSerie);
        FunkoNumeroBoxValidation = RegisterFieldValidation(nameof(FunkoNumeroBoxTexto), ValidateFunkoNumeroBox);
        FunkoLicenciaValidation = RegisterFieldValidation(nameof(FunkoLicencia), ValidateFunkoLicencia);
        ToyEdadMinimaValidation = RegisterFieldValidation(nameof(ToyEdadMinimaTexto), ValidateToyEdadMinima);
        ToyJugadoresMinValidation = RegisterFieldValidation(nameof(ToyJugadoresMinTexto), ValidateToyJugadoresMin);
        ToyJugadoresMaxValidation = RegisterFieldValidation(nameof(ToyJugadoresMaxTexto), ValidateToyJugadoresMax);
        VariosMarcaValidation = RegisterFieldValidation(nameof(VariosMarca), ValidateVariosMarca);
        VariosAltoValidation = RegisterFieldValidation(nameof(VariosAltoTexto), ValidateVariosAlto);
        VariosAnchoValidation = RegisterFieldValidation(nameof(VariosAnchoTexto), ValidateVariosAncho);
        VariosLargoValidation = RegisterFieldValidation(nameof(VariosLargoTexto), ValidateVariosLargo);
        VariosMaterialValidation = RegisterFieldValidation(nameof(VariosMaterial), ValidateVariosMaterial);

        TipoSeleccionado = TiposDisponibles[0];

        ErrorsChanged += (_, _) => GuardarCommand.NotifyCanExecuteChanged();
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(IsBusy) or nameof(CatalogosCargados))
                GuardarCommand.NotifyCanExecuteChanged();
        };

        _ = CargarCatalogosAsync();
    }

    partial void OnTipoSeleccionadoChanged(TipoProductoFiltroItem? value)
    {
        ClearFieldValidations(GetCamposPorTipo());

        OnPropertyChanged(nameof(MostrarHotWheels));
        OnPropertyChanged(nameof(MostrarFunko));
        OnPropertyChanged(nameof(MostrarTcg));
        OnPropertyChanged(nameof(MostrarToy));
        OnPropertyChanged(nameof(MostrarVarios));
    }

    partial void OnTcgFranquiciaSeleccionadaChanged(CatalogoItemDto? value)
    {
        if (value is null)
        {
            TcgExpansionesDisponibles.Clear();
            TcgPacksDisponibles.Clear();
            return;
        }

        _ = CargarExpansionesYPacksAsync(value.Id);
    }

    private async Task CargarCatalogosAsync()
    {
        try
        {
            IsBusy = true;
            var catalogos = await _mediator.Send(new ObtenerCatalogosQuery());

            foreach (var cat in catalogos.CategoriasHotWheels)
                CategoriasHotWheels.Add(cat);
            foreach (var sub in catalogos.SubtiposFunko)
                SubtiposFunko.Add(sub);
            foreach (var car in catalogos.CaracteristicasFunko)
                CaracteristicasFunko.Add(car);
            foreach (var fra in catalogos.FranquiciasTcg)
                FranquiciasTcg.Add(fra);

            CatalogosCargados = true;
        }
        catch (Exception ex)
        {
            await _notification.ShowErrorAsync("Error al cargar catalogos: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CargarExpansionesYPacksAsync(int franquiciaId)
    {
        try
        {
            var result = await _mediator.Send(new ObtenerExpansionesYPacksQuery(franquiciaId));

            TcgExpansionesDisponibles.Clear();
            foreach (var exp in result.Expansiones)
                TcgExpansionesDisponibles.Add(exp);

            TcgPacksDisponibles.Clear();
            foreach (var pack in result.Packs)
                TcgPacksDisponibles.Add(pack);
        }
        catch (Exception ex)
        {
            await _notification.ShowErrorAsync("Error al cargar expansiones: " + ex.Message);
        }
    }

    private bool ValidarFormulario() => ValidateFields(GetCamposActivos());

    private IEnumerable<string> GetCamposActivos()
    {
        yield return nameof(Nombre);
        yield return nameof(PrecioTexto);
        yield return nameof(UnidadesTexto);

        if (MostrarHotWheels)
        {
            yield return nameof(HwModelo);
            yield return nameof(HwAnioTexto);
            yield return nameof(HwSerie);
        }

        if (MostrarFunko)
        {
            yield return nameof(FunkoNumeroBoxTexto);
            yield return nameof(FunkoLicencia);
        }

        if (MostrarToy)
        {
            yield return nameof(ToyEdadMinimaTexto);
            yield return nameof(ToyJugadoresMinTexto);
            yield return nameof(ToyJugadoresMaxTexto);
        }

        if (MostrarVarios)
        {
            yield return nameof(VariosMarca);
            yield return nameof(VariosAltoTexto);
            yield return nameof(VariosAnchoTexto);
            yield return nameof(VariosLargoTexto);
            yield return nameof(VariosMaterial);
        }
    }

    private static IEnumerable<string> GetCamposPorTipo()
    {
        yield return nameof(HwModelo);
        yield return nameof(HwAnioTexto);
        yield return nameof(HwSerie);
        yield return nameof(FunkoNumeroBoxTexto);
        yield return nameof(FunkoLicencia);
        yield return nameof(ToyEdadMinimaTexto);
        yield return nameof(ToyJugadoresMinTexto);
        yield return nameof(ToyJugadoresMaxTexto);
        yield return nameof(VariosMarca);
        yield return nameof(VariosAltoTexto);
        yield return nameof(VariosAnchoTexto);
        yield return nameof(VariosLargoTexto);
        yield return nameof(VariosMaterial);
    }

    private string? ValidateNombre()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
            return "*Nombre invalido";

        return Nombre.Trim().Length > _validationRules.NombreMaxLength
            ? $"*El nombre no puede superar los {_validationRules.NombreMaxLength} caracteres"
            : null;
    }

    private string? ValidatePrecio()
    {
        if (string.IsNullOrWhiteSpace(PrecioTexto) ||
            !Regex.IsMatch(PrecioTexto.Trim(), _validationRules.PrecioFormatoPattern, RegexOptions.CultureInvariant))
            return _validationRules.PrecioFormatoMensaje;

        return ParsePrecio() < _validationRules.PrecioMinimo
            ? "*Precio invalido"
            : null;
    }

    private decimal ParsePrecio() => decimal.Parse(
        PrecioTexto!.Trim(),
        NumberStyles.AllowDecimalPoint,
        CultureInfo.InvariantCulture);

    private string? ValidateUnidades() => !TryParseInt(UnidadesTexto, out var unidades) ||
                                          unidades < _validationRules.UnidadesMinimas
        ? "*Unidades invalidas"
        : null;

    private string? ValidateHwModelo() => ValidateTextoDetalle(HwModelo, "*Modelo invalido");

    private string? ValidateHwAnio() => !TryParseInt(HwAnioTexto, out var anio) ||
                                        anio < _validationRules.HotWheelsAnioMinimo ||
                                        anio > _validationRules.HotWheelsAnioMaximo
        ? "*Anio invalido"
        : null;

    private string? ValidateHwSerie() => ValidateTextoDetalle(HwSerie, "*Serie invalida");

    private string? ValidateFunkoNumeroBox() => !TryParseInt(FunkoNumeroBoxTexto, out var numeroBox) ||
                                                numeroBox < _validationRules.FunkoNumeroCajaMinimo
        ? "*Numero de caja invalido"
        : null;

    private string? ValidateFunkoLicencia() => ValidateTextoDetalle(FunkoLicencia, "*Licencia invalida");

    private string? ValidateToyEdadMinima() => !TryParseInt(ToyEdadMinimaTexto, out var edadMinima) || edadMinima < 0
        ? "*Edad minima invalida"
        : null;

    private string? ValidateToyJugadoresMin() => !TryParseInt(ToyJugadoresMinTexto, out var jugadoresMin) ||
                                                jugadoresMin < _validationRules.ToyJugadoresMinimo
        ? "*Jugadores minimos invalido"
        : null;

    private string? ValidateToyJugadoresMax() => !TryParseInt(ToyJugadoresMinTexto, out var jugadoresMin) ||
                                                !TryParseInt(ToyJugadoresMaxTexto, out var jugadoresMax) ||
                                                jugadoresMax < jugadoresMin
        ? "*Jugadores maximos invalido"
        : null;

    private string? ValidateVariosMarca() => ValidateTextoDetalle(VariosMarca, "*Marca invalida");

    private string? ValidateVariosAlto() => !TryParseDecimal(VariosAltoTexto, out var alto) ||
                                           alto < _validationRules.DimensionMinima
        ? "*Alto invalido"
        : null;

    private string? ValidateVariosAncho() => !TryParseDecimal(VariosAnchoTexto, out var ancho) ||
                                            ancho < _validationRules.DimensionMinima
        ? "*Ancho invalido"
        : null;

    private string? ValidateVariosLargo() => string.IsNullOrWhiteSpace(VariosLargoTexto) ||
                                            TryParseDecimal(VariosLargoTexto, out _)
        ? null
        : "*Largo invalido";

    private string? ValidateVariosMaterial() => ValidateTextoDetalle(VariosMaterial, "*Material invalido");

    private string? ValidateTextoDetalle(string? value, string invalidMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            return invalidMessage;

        return value.Trim().Length > _validationRules.TextoDetalleMaxLength
            ? $"*No puede superar los {_validationRules.TextoDetalleMaxLength} caracteres"
            : null;
    }

    private static bool TryParseInt(string? value, out int result) =>
        int.TryParse(value?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out result);

    private static int ParseInt(string? value) =>
        int.Parse(value!.Trim(), NumberStyles.None, CultureInfo.InvariantCulture);

    private static bool TryParseDecimal(string? value, out decimal result) =>
        decimal.TryParse(value?.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);

    private static decimal ParseDecimal(string? value) =>
        decimal.Parse(value!.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

    private static decimal? ParseOptionalDecimal(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : ParseDecimal(value);

    partial void OnNombreChanged(string? value) => ClearFieldValidation(nameof(Nombre));
    partial void OnPrecioTextoChanged(string? value) => ClearFieldValidation(nameof(PrecioTexto));
    partial void OnUnidadesTextoChanged(string? value) => ClearFieldValidation(nameof(UnidadesTexto));
    partial void OnHwModeloChanged(string? value) => ClearFieldValidation(nameof(HwModelo));
    partial void OnHwAnioTextoChanged(string? value) => ClearFieldValidation(nameof(HwAnioTexto));
    partial void OnHwSerieChanged(string? value) => ClearFieldValidation(nameof(HwSerie));
    partial void OnFunkoNumeroBoxTextoChanged(string? value) => ClearFieldValidation(nameof(FunkoNumeroBoxTexto));
    partial void OnFunkoLicenciaChanged(string? value) => ClearFieldValidation(nameof(FunkoLicencia));
    partial void OnToyEdadMinimaTextoChanged(string? value) => ClearFieldValidation(nameof(ToyEdadMinimaTexto));

    partial void OnToyJugadoresMinTextoChanged(string? value)
    {
        ClearFieldValidation(nameof(ToyJugadoresMinTexto));
        ClearFieldValidation(nameof(ToyJugadoresMaxTexto));
    }

    partial void OnToyJugadoresMaxTextoChanged(string? value) => ClearFieldValidation(nameof(ToyJugadoresMaxTexto));
    partial void OnVariosMarcaChanged(string? value) => ClearFieldValidation(nameof(VariosMarca));
    partial void OnVariosAltoTextoChanged(string? value) => ClearFieldValidation(nameof(VariosAltoTexto));
    partial void OnVariosAnchoTextoChanged(string? value) => ClearFieldValidation(nameof(VariosAnchoTexto));
    partial void OnVariosLargoTextoChanged(string? value) => ClearFieldValidation(nameof(VariosLargoTexto));
    partial void OnVariosMaterialChanged(string? value) => ClearFieldValidation(nameof(VariosMaterial));

    [RelayCommand(CanExecute = nameof(CanGuardar))]
    private async Task GuardarAsync()
    {
        if (IsBusy) return;

        if (!ValidarFormulario())
            return;

        var confirmar = await _confirmation.ConfirmAsync("¿Desea crear el producto?");
        if (!confirmar) return;

        try
        {
            IsBusy = true;
            var command = ConstruirCommand();
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                await _notification.ShowSuccessAsync("Producto creado exitosamente.");
                _navigation.NavigateToMenu();
            }
            else
            {
                await _notification.ShowErrorAsync(result.Error ?? "Error al crear producto.");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private CrearProductoCommand ConstruirCommand() => new()
    {
        Nombre = Nombre!,
        Precio = ParsePrecio(),
        Unidades = ParseInt(UnidadesTexto),
        Tipo = TipoSeleccionado!.Valor!.Value,

        HotWheels = MostrarHotWheels && HwCategoriaSeleccionada is not null
            ? new CrearHotWheelsDetalleDto(HwModelo!, ParseInt(HwAnioTexto), HwSerie!, HwCategoriaSeleccionada.Id)
            : null,

        Funko = MostrarFunko && FunkoSubtipoSeleccionado is not null
            ? new CrearFunkoDetalleDto(
                ParseInt(FunkoNumeroBoxTexto), FunkoLicencia!,
                FunkoSubtipoSeleccionado.Id,
                FunkoCaracteristicaSeleccionada?.Id)
            : null,

        Tcg = MostrarTcg && TcgPackSeleccionado is not null && TcgExpansionSeleccionada is not null
            ? new CrearTcgDetalleDto(TcgPackSeleccionado.Id, TcgExpansionSeleccionada.Id)
            : null,

        Toy = MostrarToy
            ? new CrearToyDetalleDto(
                ParseInt(ToyEdadMinimaTexto),
                ParseInt(ToyJugadoresMinTexto),
                ParseInt(ToyJugadoresMaxTexto),
                ToyEsJuegoMesa)
            : null,

        Varios = MostrarVarios
            ? new CrearVariosDetalleDto(
                VariosMarca!, ParseDecimal(VariosAltoTexto), ParseDecimal(VariosAnchoTexto),
                ParseOptionalDecimal(VariosLargoTexto), VariosMaterial!, VariosTieneIlustracion)
            : null
    };

    [RelayCommand]
    private void Volver() => _navigation.NavigateToMenu();

    private bool CanGuardar() => !HasErrors && !IsBusy && CatalogosCargados;
}
