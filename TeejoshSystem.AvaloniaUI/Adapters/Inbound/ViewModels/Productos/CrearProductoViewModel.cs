using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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

    // ─── Campos comunes ───────────────────────────────────────────────────

    [ObservableProperty] private string? _nombre;
    [ObservableProperty] private string? _precioTexto = "0.00";
    [ObservableProperty] private int _unidades;
    [ObservableProperty] private TipoProductoFiltroItem? _tipoSeleccionado;
    [ObservableProperty] private bool _catalogosCargados;

    // ─── Estado visual de validación ──────────────────────────────────────

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
    public FieldValidationState VariosMarcaValidation { get; }
    public FieldValidationState VariosAltoValidation { get; }
    public FieldValidationState VariosAnchoValidation { get; }
    public FieldValidationState VariosMaterialValidation { get; }

    // ─── Hot Wheels ───────────────────────────────────────────────────────

    [ObservableProperty] private string? _hwModelo;
    [ObservableProperty] private int _hwAnio = DateTime.Now.Year;
    [ObservableProperty] private string? _hwSerie;
    [ObservableProperty] private CatalogoItemDto? _hwCategoriaSeleccionada;

    public ObservableCollection<CatalogoItemDto> CategoriasHotWheels { get; } = new();

    // ─── Funko ────────────────────────────────────────────────────────────

    [ObservableProperty] private int _funkoNumeroBox;
    [ObservableProperty] private string? _funkoLicencia;
    [ObservableProperty] private CatalogoItemDto? _funkoSubtipoSeleccionado;
    [ObservableProperty] private CatalogoItemDto? _funkoCaracteristicaSeleccionada;

    public ObservableCollection<CatalogoItemDto> SubtiposFunko { get; } = new();
    public ObservableCollection<CatalogoItemDto> CaracteristicasFunko { get; } = new();

    // ─── TCG ──────────────────────────────────────────────────────────────

    [ObservableProperty] private CatalogoItemDto? _tcgFranquiciaSeleccionada;
    [ObservableProperty] private CatalogoItemDto? _tcgExpansionSeleccionada;
    [ObservableProperty] private CatalogoItemDto? _tcgPackSeleccionado;

    public ObservableCollection<CatalogoItemDto> FranquiciasTcg { get; } = new();
    public ObservableCollection<CatalogoItemDto> TcgExpansionesDisponibles { get; } = new();
    public ObservableCollection<CatalogoItemDto> TcgPacksDisponibles { get; } = new();

    // ─── Toy ──────────────────────────────────────────────────────────────

    [ObservableProperty] private int _toyEdadMinima;
    [ObservableProperty] private int _toyJugadoresMin = 1;
    [ObservableProperty] private int _toyJugadoresMax = 1;
    [ObservableProperty] private bool _toyEsJuegoMesa;

    // ─── Varios ───────────────────────────────────────────────────────────

    [ObservableProperty] private string? _variosMarca;
    [ObservableProperty] private decimal _variosAlto;
    [ObservableProperty] private decimal _variosAncho;
    [ObservableProperty] private decimal? _variosLargo;
    [ObservableProperty] private string? _variosMaterial;
    [ObservableProperty] private bool _variosTieneIlustracion;

    // ─── Visibilidad de paneles ───────────────────────────────────────────

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

    // ─── Constructor ──────────────────────────────────────────────────────

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
        UnidadesValidation = RegisterFieldValidation(nameof(Unidades), ValidateUnidades);
        HwModeloValidation = RegisterFieldValidation(nameof(HwModelo), ValidateHwModelo);
        HwAnioValidation = RegisterFieldValidation(nameof(HwAnio), ValidateHwAnio);
        HwSerieValidation = RegisterFieldValidation(nameof(HwSerie), ValidateHwSerie);
        FunkoNumeroBoxValidation = RegisterFieldValidation(nameof(FunkoNumeroBox), ValidateFunkoNumeroBox);
        FunkoLicenciaValidation = RegisterFieldValidation(nameof(FunkoLicencia), ValidateFunkoLicencia);
        ToyJugadoresMinValidation = RegisterFieldValidation(nameof(ToyJugadoresMin), ValidateToyJugadoresMin);
        ToyJugadoresMaxValidation = RegisterFieldValidation(nameof(ToyJugadoresMax), ValidateToyJugadoresMax);
        VariosMarcaValidation = RegisterFieldValidation(nameof(VariosMarca), ValidateVariosMarca);
        VariosAltoValidation = RegisterFieldValidation(nameof(VariosAlto), ValidateVariosAlto);
        VariosAnchoValidation = RegisterFieldValidation(nameof(VariosAncho), ValidateVariosAncho);
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

    // ─── Cambios de tipo / franquicia ─────────────────────────────────────

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

    // ─── Carga de catálogos ───────────────────────────────────────────────

    private async Task CargarCatalogosAsync()
    {
        try
        {
            IsBusy = true;
            var catalogos = await _mediator.Send(new ObtenerCatalogosQuery());

            foreach (var c in catalogos.CategoriasHotWheels) CategoriasHotWheels.Add(c);
            foreach (var s in catalogos.SubtiposFunko) SubtiposFunko.Add(s);
            foreach (var c in catalogos.CaracteristicasFunko) CaracteristicasFunko.Add(c);
            foreach (var f in catalogos.FranquiciasTcg) FranquiciasTcg.Add(f);

            CatalogosCargados = true;
        }
        catch (Exception ex)
        {
            await _notification.ShowErrorAsync("Error al cargar catálogos: " + ex.Message);
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
            foreach (var e in result.Expansiones) TcgExpansionesDisponibles.Add(e);

            TcgPacksDisponibles.Clear();
            foreach (var p in result.Packs) TcgPacksDisponibles.Add(p);
        }
        catch (Exception ex)
        {
            await _notification.ShowErrorAsync("Error al cargar expansiones: " + ex.Message);
        }
    }

    // ─── Validación: limpiar al entrar/escribir, validar al salir ─────────

    private bool ValidarFormulario() => ValidateFields(GetCamposActivos());

    private IEnumerable<string> GetCamposActivos()
    {
        yield return nameof(Nombre);
        yield return nameof(PrecioTexto);
        yield return nameof(Unidades);

        if (MostrarHotWheels)
        {
            yield return nameof(HwModelo);
            yield return nameof(HwAnio);
            yield return nameof(HwSerie);
        }

        if (MostrarFunko)
        {
            yield return nameof(FunkoNumeroBox);
            yield return nameof(FunkoLicencia);
        }

        if (MostrarToy)
        {
            yield return nameof(ToyJugadoresMin);
            yield return nameof(ToyJugadoresMax);
        }

        if (MostrarVarios)
        {
            yield return nameof(VariosMarca);
            yield return nameof(VariosAlto);
            yield return nameof(VariosAncho);
            yield return nameof(VariosMaterial);
        }
    }

    private static IEnumerable<string> GetCamposPorTipo()
    {
        yield return nameof(HwModelo);
        yield return nameof(HwAnio);
        yield return nameof(HwSerie);
        yield return nameof(FunkoNumeroBox);
        yield return nameof(FunkoLicencia);
        yield return nameof(ToyJugadoresMin);
        yield return nameof(ToyJugadoresMax);
        yield return nameof(VariosMarca);
        yield return nameof(VariosAlto);
        yield return nameof(VariosAncho);
        yield return nameof(VariosMaterial);
    }

    private string? ValidateNombre()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
            return "*Nombre inválido";

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
            ? "*Precio inválido"
            : null;
    }

    private decimal ParsePrecio() => decimal.Parse(
        PrecioTexto!.Trim(),
        NumberStyles.AllowDecimalPoint,
        CultureInfo.InvariantCulture);

    private string? ValidateUnidades() => Unidades < _validationRules.UnidadesMinimas
        ? "*Unidades inválidas"
        : null;

    private string? ValidateHwModelo() => ValidateTextoDetalle(HwModelo, "*Modelo inválido");

    private string? ValidateHwAnio() => HwAnio < _validationRules.HotWheelsAnioMinimo ||
                                        HwAnio > _validationRules.HotWheelsAnioMaximo
        ? "*Año inválido"
        : null;

    private string? ValidateHwSerie() => ValidateTextoDetalle(HwSerie, "*Serie inválida");

    private string? ValidateFunkoNumeroBox() => FunkoNumeroBox < _validationRules.FunkoNumeroCajaMinimo
        ? "*Número de caja inválido"
        : null;

    private string? ValidateFunkoLicencia() => ValidateTextoDetalle(FunkoLicencia, "*Licencia inválida");

    private string? ValidateToyJugadoresMin() => ToyJugadoresMin < _validationRules.ToyJugadoresMinimo
        ? "*Jugadores mínimos inválido"
        : null;

    private string? ValidateToyJugadoresMax() => ToyJugadoresMax < ToyJugadoresMin
        ? "*Jugadores máximos inválido"
        : null;

    private string? ValidateVariosMarca() => ValidateTextoDetalle(VariosMarca, "*Marca inválida");

    private string? ValidateVariosAlto() => VariosAlto < _validationRules.DimensionMinima
        ? "*Alto inválido"
        : null;

    private string? ValidateVariosAncho() => VariosAncho < _validationRules.DimensionMinima
        ? "*Ancho inválido"
        : null;

    private string? ValidateVariosMaterial() => ValidateTextoDetalle(VariosMaterial, "*Material inválido");

    private string? ValidateTextoDetalle(string? value, string invalidMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            return invalidMessage;

        return value.Trim().Length > _validationRules.TextoDetalleMaxLength
            ? $"*No puede superar los {_validationRules.TextoDetalleMaxLength} caracteres"
            : null;
    }

    partial void OnNombreChanged(string? value) => ClearFieldValidation(nameof(Nombre));
    partial void OnPrecioTextoChanged(string? value) => ClearFieldValidation(nameof(PrecioTexto));
    partial void OnUnidadesChanged(int value) => ClearFieldValidation(nameof(Unidades));
    partial void OnHwModeloChanged(string? value) => ClearFieldValidation(nameof(HwModelo));
    partial void OnHwAnioChanged(int value) => ClearFieldValidation(nameof(HwAnio));
    partial void OnHwSerieChanged(string? value) => ClearFieldValidation(nameof(HwSerie));
    partial void OnFunkoNumeroBoxChanged(int value) => ClearFieldValidation(nameof(FunkoNumeroBox));
    partial void OnFunkoLicenciaChanged(string? value) => ClearFieldValidation(nameof(FunkoLicencia));
    partial void OnToyJugadoresMinChanged(int value)
    {
        ClearFieldValidation(nameof(ToyJugadoresMin));
        ClearFieldValidation(nameof(ToyJugadoresMax));
    }

    partial void OnToyJugadoresMaxChanged(int value) => ClearFieldValidation(nameof(ToyJugadoresMax));
    partial void OnVariosMarcaChanged(string? value) => ClearFieldValidation(nameof(VariosMarca));
    partial void OnVariosAltoChanged(decimal value) => ClearFieldValidation(nameof(VariosAlto));
    partial void OnVariosAnchoChanged(decimal value) => ClearFieldValidation(nameof(VariosAncho));
    partial void OnVariosMaterialChanged(string? value) => ClearFieldValidation(nameof(VariosMaterial));

    // ─── Guardar ──────────────────────────────────────────────────────────

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

    private bool CanGuardar() => !HasErrors && !IsBusy && CatalogosCargados;

    private CrearProductoCommand ConstruirCommand() => new()
    {
        Nombre = Nombre!,
        Precio = ParsePrecio(),
        Unidades = Unidades,
        Tipo = TipoSeleccionado!.Valor!.Value,

        HotWheels = MostrarHotWheels && HwCategoriaSeleccionada is not null
            ? new CrearHotWheelsDetalleDto(HwModelo!, HwAnio, HwSerie!, HwCategoriaSeleccionada.Id)
            : null,

        Funko = MostrarFunko && FunkoSubtipoSeleccionado is not null
            ? new CrearFunkoDetalleDto(
                FunkoNumeroBox, FunkoLicencia!,
                FunkoSubtipoSeleccionado.Id,
                FunkoCaracteristicaSeleccionada?.Id)
            : null,

        Tcg = MostrarTcg && TcgPackSeleccionado is not null && TcgExpansionSeleccionada is not null
            ? new CrearTcgDetalleDto(TcgPackSeleccionado.Id, TcgExpansionSeleccionada.Id)
            : null,

        Toy = MostrarToy
            ? new CrearToyDetalleDto(ToyEdadMinima, ToyJugadoresMin, ToyJugadoresMax, ToyEsJuegoMesa)
            : null,

        Varios = MostrarVarios
            ? new CrearVariosDetalleDto(
                VariosMarca!, VariosAlto, VariosAncho,
                VariosLargo, VariosMaterial!, VariosTieneIlustracion)
            : null
    };

    [RelayCommand]
    private void Volver() => _navigation.NavigateToMenu();
}
