using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Menu;

/// <summary>Item de selector de tema para binding en ComboBox.</summary>
public record TemaItem(string Nombre, ThemeMode Modo);

public partial class MenuPrincipalViewModel : ViewModelBase
{
    private readonly IServiceProvider   _serviceProvider;
    private readonly INavigationService _navigation;
    private readonly IThemeService      _themeService;

    // ─── Tema ────────────────────────────────────────────────────────────

    public ObservableCollection<TemaItem> TemasDisponibles { get; } = new()
    {
        new TemaItem("☀  Claro",  ThemeMode.Light),
        new TemaItem("🌙 Oscuro", ThemeMode.Dark),
        new TemaItem("⚙  Sistema", ThemeMode.System)
    };

    [ObservableProperty]
    private TemaItem? _temaSeleccionado;

    partial void OnTemaSeleccionadoChanged(TemaItem? value)
    {
        if (value is not null)
            _themeService.Apply(value.Modo);
    }

    // ─── Constructor ─────────────────────────────────────────────────────

    public MenuPrincipalViewModel(
        IServiceProvider   serviceProvider,
        INavigationService navigation)
    {
        _serviceProvider = serviceProvider;
        _navigation      = navigation;

        // IThemeService se resuelve desde el contenedor para no cambiar
        // la firma del constructor (compatibilidad con el wiring de App.axaml.cs)
        _themeService = serviceProvider.GetRequiredService<IThemeService>();

        // Sincroniza el ComboBox con el tema actual
        _temaSeleccionado = TemasDisponibles[(int)_themeService.CurrentMode];
    }

    // ─── Comandos de navegación ───────────────────────────────────────────

    [RelayCommand]
    private void VisualizarInventario()
    {
        var vm = new InventarioViewModel(
            _serviceProvider.GetRequiredService<IMediator>(),
            _navigation.NavigateToMenu);
        _navigation.NavigateTo(vm);
    }

    [RelayCommand]
    private void ModificarProducto()
    {
        var vm = _serviceProvider.GetRequiredService<GestionarProductosViewModel>();
        _navigation.NavigateTo(vm);
    }

    [RelayCommand]
    private void AnadirProducto()
    {
        var vm = _serviceProvider.GetRequiredService<CrearProductoViewModel>();
        _navigation.NavigateTo(vm);
    }
}