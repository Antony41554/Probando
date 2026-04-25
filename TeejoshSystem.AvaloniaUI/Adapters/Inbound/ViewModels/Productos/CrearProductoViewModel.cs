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

    // Propiedades comunes
    [ObservableProperty]
    private string? _nombre;

    [ObservableProperty]
    private decimal _precio;

    [ObservableProperty]
    private int _unidades;

    [ObservableProperty]
    private TipoProductoFiltroItem? _tipoSeleccionado;

    [ObservableProperty]
    private bool _catalogosCargados;

    // Hot Wheels
    [ObservableProperty]
    private string? _hwModelo;

    [ObservableProperty]
    private int _hwAnio = DateTime.Now.Year;

    [ObservableProperty]
    private string? _hwSerie;

    [ObservableProperty]
    private CatalogoItemDto? _hwCategoriaSeleccionada;

    public ObservableCollection<CatalogoItemDto> CategoriasHotWheels { get; } = new();

    // Funko
    [ObservableProperty]
    private int _funkoNumeroBox;

    [ObservableProperty]
    private string? _funkoLicencia;

    [ObservableProperty]
    private CatalogoItemDto? _funkoSubtipoSeleccionado;

    [ObservableProperty]
    private CatalogoItemDto? _funkoCaracteristicaSeleccionada;

    public ObservableCollection<CatalogoItemDto> SubtiposFunko { get; } = new();
    public ObservableCollection<CatalogoItemDto> CaracteristicasFunko { get; } = new();

    // TCG
    [ObservableProperty]
    private CatalogoItemDto? _tcgFranquiciaSeleccionada;

    [ObservableProperty]
    private CatalogoItemDto? _tcgExpansionSeleccionada;

    [ObservableProperty]
    private CatalogoItemDto? _tcgPackSeleccionado;

    public ObservableCollection<CatalogoItemDto> FranquiciasTcg { get; } = new();
    public ObservableCollection<CatalogoItemDto> TcgExpansionesDisponibles { get; } = new();
    public ObservableCollection<CatalogoItemDto> TcgPacksDisponibles { get; } = new();

    // Toy
    [ObservableProperty]
    private int _toyEdadMinima;

    [ObservableProperty]
    private int _toyJugadoresMin = 1;

    [ObservableProperty]
    private int _toyJugadoresMax = 1;

    [ObservableProperty]
    private bool _toyEsJuegoMesa;

    // Varios
    [ObservableProperty]
    private string? _variosMarca;

    [ObservableProperty]
    private decimal _variosAlto;

    [ObservableProperty]
    private decimal _variosAncho;

    [ObservableProperty]
    private decimal? _variosLargo;

    [ObservableProperty]
    private string? _variosMaterial;

    [ObservableProperty]
    private bool _variosTieneIlustracion;

    // Visibilidad de paneles
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

        TipoSeleccionado = TiposDisponibles[0];

        ErrorsChanged += (_, _) => GuardarCommand.NotifyCanExecuteChanged();

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsBusy) ||
                e.PropertyName == nameof(CatalogosCargados))
                GuardarCommand.NotifyCanExecuteChanged();
        };

        _ = CargarCatalogosAsync();
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
            await _notification.ShowErrorAsync("Error al cargar catálogos: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

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

    private async Task CargarExpansionesYPacksAsync(int franquiciaId)
    {
        try
        {
            var result = await _mediator.Send(
                new ObtenerExpansionesYPacksQuery(franquiciaId));

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

    private CrearProductoCommand ConstruirCommand() => new()
    {
        Nombre = Nombre!,
        Precio = Precio,
        Unidades = Unidades,
        Tipo = TipoSeleccionado!.Valor!.Value,

        HotWheels = MostrarHotWheels && HwCategoriaSeleccionada != null
            ? new CrearHotWheelsDetalleDto(HwModelo!, HwAnio, HwSerie!, HwCategoriaSeleccionada.Id)
            : null,

        Funko = MostrarFunko && FunkoSubtipoSeleccionado != null
            ? new CrearFunkoDetalleDto(
                FunkoNumeroBox, FunkoLicencia!,
                FunkoSubtipoSeleccionado.Id,
                FunkoCaracteristicaSeleccionada?.Id)
            : null,

        Tcg = MostrarTcg && TcgPackSeleccionado != null && TcgExpansionSeleccionada != null
            ? new CrearTcgDetalleDto(TcgPackSeleccionado.Id, TcgExpansionSeleccionada.Id)
            : null,

        Toy = MostrarToy
            ? new CrearToyDetalleDto(
                ToyEdadMinima, ToyJugadoresMin, ToyJugadoresMax, ToyEsJuegoMesa)
            : null,

        Varios = MostrarVarios
            ? new CrearVariosDetalleDto(
                VariosMarca!, VariosAlto, VariosAncho,
                VariosLargo, VariosMaterial!, VariosTieneIlustracion)
            : null
    };

    [RelayCommand]
    private void Volver() => _navigation.NavigateToMenu();

    private bool CanGuardar() => !HasErrors && !IsBusy && CatalogosCargados;

    partial void OnNombreChanged(string? value)
    {
        ClearErrors(nameof(Nombre));
        if (string.IsNullOrWhiteSpace(value))
            AddError(nameof(Nombre), "El nombre es obligatorio.");
        else if (value.Length > 50)
            AddError(nameof(Nombre), "Máximo 50 caracteres.");
    }

    partial void OnPrecioChanged(decimal value)
    {
        ClearErrors(nameof(Precio));
        if (value < 0)
            AddError(nameof(Precio), "El precio no puede ser negativo.");
    }

    partial void OnUnidadesChanged(int value)
    {
        ClearErrors(nameof(Unidades));
        if (value < 0)
            AddError(nameof(Unidades), "Las unidades no pueden ser negativas.");
    }
}