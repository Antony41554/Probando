using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System;
using System.Collections.ObjectModel;
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

    // ─── Campos comunes ───────────────────────────────────────────────────

    [ObservableProperty] private string? _nombre;
    [ObservableProperty] private decimal _precio;
    [ObservableProperty] private int _unidades;
    [ObservableProperty] private TipoProductoFiltroItem? _tipoSeleccionado;
    [ObservableProperty] private bool _catalogosCargados;

    // ─── Errores bindeables ───────────────────────────────────────────────

    public string? NombreError => GetFirstError(nameof(Nombre));
    public string? PrecioError => GetFirstError(nameof(Precio));
    public string? UnidadesError => GetFirstError(nameof(Unidades));

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

    // ─── Validación: limpiar al escribir, validar al salir del campo ──────

    /// <summary>
    /// Invocado desde EventTriggerBehavior (LostFocus) en cada campo del XAML.
    /// Recibe el nombre de la propiedad como CommandParameter.
    /// </summary>
    [RelayCommand]
    private void ValidarCampo(string campo)
    {
        switch (campo)
        {
            case nameof(Nombre):
                ClearErrors(nameof(Nombre));
                if (string.IsNullOrWhiteSpace(Nombre))
                    AddError(nameof(Nombre), "*Nombre inválido.");
                else if (Nombre.Length > 100)
                    AddError(nameof(Nombre), "*El nombre no puede superar los 100 caracteres.");
                OnPropertyChanged(nameof(NombreError));
                break;

            case nameof(Precio):
                ClearErrors(nameof(Precio));
                if (Precio < 0)
                    AddError(nameof(Precio), "*Precio inválido.");
                OnPropertyChanged(nameof(PrecioError));
                break;

            case nameof(Unidades):
                ClearErrors(nameof(Unidades));
                if (Unidades < 0)
                    AddError(nameof(Unidades), "*Unidades inválidas.");
                OnPropertyChanged(nameof(UnidadesError));
                break;
        }
    }

    // OnXxxChanged solo limpia — el usuario ve el error desaparecer al empezar a corregir

    partial void OnNombreChanged(string? value)
    {
        ClearErrors(nameof(Nombre));
        OnPropertyChanged(nameof(NombreError));
    }

    partial void OnPrecioChanged(decimal value)
    {
        ClearErrors(nameof(Precio));
        OnPropertyChanged(nameof(PrecioError));
    }

    partial void OnUnidadesChanged(int value)
    {
        ClearErrors(nameof(Unidades));
        OnPropertyChanged(nameof(UnidadesError));
    }

    // ─── Guardar ──────────────────────────────────────────────────────────

    [RelayCommand(CanExecute = nameof(CanGuardar))]
    private async Task GuardarAsync()
    {
        if (IsBusy) return;

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
        Precio = Precio,
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