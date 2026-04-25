using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using TeejoshSystem.Application.Ports.Inbound.Productos.Commands.EliminarProducto;
using TeejoshSystem.Application.Ports.Inbound.Productos.Queries.BuscarProductos;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;
using TeejoshSystem.Domain.Enums;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

public partial class GestionarProductosViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly IConfirmationService _confirmation;
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private string? _textoBusqueda;

    [ObservableProperty]
    private TipoProductoFiltroItem? _tipoFiltro;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditarCommand))]
    [NotifyCanExecuteChangedFor(nameof(EliminarCommand))]
    private ProductoBusquedaDto? _productoSeleccionado;

    public ObservableCollection<ProductoBusquedaDto> Productos { get; } = new();

    public ObservableCollection<TipoProductoFiltroItem> TiposDisponibles { get; } = new()
    {
        new TipoProductoFiltroItem("Todos", null),
        new TipoProductoFiltroItem("Hot Wheels", TipoProducto.HotWheels),
        new TipoProductoFiltroItem("Funko", TipoProducto.Funko),
        new TipoProductoFiltroItem("TCG", TipoProducto.Tcg),
        new TipoProductoFiltroItem("Toy", TipoProducto.Toy),
        new TipoProductoFiltroItem("Varios", TipoProducto.Varios)
    };

    public GestionarProductosViewModel(
        IMediator mediator,
        INotificationService notification,
        IConfirmationService confirmation,
        INavigationService navigation)
    {
        _mediator = mediator;
        _notification = notification;
        _confirmation = confirmation;
        _navigation = navigation;

        _ = BuscarAsync();
    }

    [RelayCommand]
    public async Task BuscarAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var resultados = await _mediator.Send(
                new BuscarProductosQuery(TextoBusqueda, TipoFiltro?.Valor));

            Productos.Clear();
            foreach (var producto in resultados)
                Productos.Add(producto);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(HaySeleccion))]
    private void Editar()
    {
        var editarVm = new EditarProductoViewModel(
            _mediator,
            _notification,
            _confirmation,
            _navigation,
            this,               // ← GestionarProductosViewModel
            ProductoSeleccionado!.Id,
            ProductoSeleccionado!.Tipo);

        _navigation.NavigateTo(editarVm);
    }

    [RelayCommand(CanExecute = nameof(HaySeleccion))]
    private async Task EliminarAsync()
    {
        var confirmar = await _confirmation.ConfirmAsync(
            $"¿Desea eliminar '{ProductoSeleccionado!.Nombre}'?");
        if (!confirmar) return;

        try
        {
            IsBusy = true;

            var result = await _mediator.Send(
                new EliminarProductosCommand(new List<int> { ProductoSeleccionado!.Id }));

            if (result.IsSuccess)
            {
                await _notification.ShowSuccessAsync("Producto eliminado correctamente.");
                ProductoSeleccionado = null;
                await BuscarAsync();
            }
            else
            {
                await _notification.ShowErrorAsync(result.Error ?? "Error al eliminar.");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Volver() => _navigation.NavigateToMenu();

    private bool HaySeleccion() => ProductoSeleccionado is not null;
}