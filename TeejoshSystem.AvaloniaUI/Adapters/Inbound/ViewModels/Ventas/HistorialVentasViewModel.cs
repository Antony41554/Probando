using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using TeejoshSystem.Application.Common.Dtos;
using TeejoshSystem.Application.Ports.Inbound.Ventas.Queries.ObtenerVentas;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Ventas;

public partial class HistorialVentasViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private DateTime? _fechaDesde;

    [ObservableProperty]
    private DateTime? _fechaHasta;

    [ObservableProperty]
    private VentaDto? _ventaSeleccionada;

    public ObservableCollection<VentaDto> Ventas { get; } = new();

    public HistorialVentasViewModel(
        IMediator mediator,
        INotificationService notification,
        INavigationService navigation)
    {
        _mediator = mediator;
        _notification = notification;
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
            var ventas = await _mediator.Send(
                new ObtenerVentasQuery(FechaDesde, FechaHasta));

            Ventas.Clear();
            foreach (var v in ventas)
                Ventas.Add(v);
        }
        catch (Exception ex)
        {
            await _notification.ShowErrorAsync("Error al cargar historial: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void LimpiarFiltros()
    {
        FechaDesde = null;
        FechaHasta = null;
        _ = BuscarAsync();
    }

    [RelayCommand]
    private void Volver() => _navigation.NavigateToMenu();
}