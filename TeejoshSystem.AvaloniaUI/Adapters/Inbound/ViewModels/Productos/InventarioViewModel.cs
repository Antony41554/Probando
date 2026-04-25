using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using TeejoshSystem.Application.Ports.Inbound.Productos.Queries.BuscarProductos;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;
using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

public partial class InventarioViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly Action _volver;

    /*
    [ObservableProperty]
    private ObservableCollection<ProductoBusquedaDto> _productos = new();
    */

    public ObservableCollection<ProductoBusquedaDto> Productos { get; } = new();

    [ObservableProperty]
    private string _busqueda = string.Empty;

    [ObservableProperty]
    private TipoProductoFiltroItem? _tipoFiltro;

    public ObservableCollection<TipoProductoFiltroItem> TiposDisponibles { get; } = new()
    {
        new TipoProductoFiltroItem("Todos", null),
        new TipoProductoFiltroItem("Hot Wheels", TipoProducto.HotWheels),
        new TipoProductoFiltroItem("Funko", TipoProducto.Funko),
        new TipoProductoFiltroItem("TCG", TipoProducto.Tcg),
        new TipoProductoFiltroItem("Toy", TipoProducto.Toy),
        new TipoProductoFiltroItem("Varios", TipoProducto.Varios)
    };

    public InventarioViewModel(IMediator mediator, Action volver)
    {
        _mediator = mediator;
        _volver = volver;
        _tipoFiltro = TiposDisponibles[0]; // "Todos" por defecto

        _ = CargarProductosAsync();
    }

    public async Task CargarProductosAsync()
    {
        IsBusy = true;

        var resultado = await _mediator.Send(new BuscarProductosQuery(
            string.IsNullOrWhiteSpace(Busqueda) ? null : Busqueda,
            TipoFiltro?.Valor
        ));

        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            Productos.Clear();
            foreach (var item in resultado)
                Productos.Add(item);

            IsBusy = false;
        });
    }

    [RelayCommand]
    private async Task Cargar() => await CargarProductosAsync();

    [RelayCommand]
    private void Volver() => _volver();
}