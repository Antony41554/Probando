using CommunityToolkit.Mvvm.Input;
using MediatR;

using Microsoft.Extensions.DependencyInjection;
using System;

using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Menu
{
    public partial class MenuPrincipalViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigation;

        public MenuPrincipalViewModel(
            IServiceProvider serviceProvider,
            INavigationService navigation)
        {
            _serviceProvider = serviceProvider;
            _navigation = navigation;
        }

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
}