using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Shell
{
    /// <summary>Item de selector de tema para binding en ComboBox.</summary>
    public record TemaItem(string Nombre, ThemeMode Modo);

    public partial class MainViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IThemeService _themeService;
        private readonly IUserSettingsService _userSettingsService;
        private bool _initializing;

        [ObservableProperty]
        private object? _currentView;

        public ObservableCollection<TemaItem> TemasDisponibles { get; } = new()
        {
            new TemaItem("⚙  Sistema", ThemeMode.System),
            new TemaItem("☀  Claro",  ThemeMode.Light),
            new TemaItem("🌙 Oscuro", ThemeMode.Dark)
        };

        [ObservableProperty]
        private TemaItem? _temaSeleccionado;

        partial void OnTemaSeleccionadoChanged(TemaItem? value)
        {
            if (value is not null && !_initializing)
            {
                _themeService.Apply(value.Modo);
                _userSettingsService.SaveTheme(value.Modo);
            }
        }

        public MainViewModel(
            IServiceProvider serviceProvider, 
            IThemeService themeService, 
            IUserSettingsService userSettingsService)
        {
            _serviceProvider = serviceProvider;
            _themeService = themeService;
            _userSettingsService = userSettingsService;
        }

        public void InitializeTheme()
        {
            _initializing = true;
            try
            {
                var loadedMode = _userSettingsService.LoadTheme();
                _themeService.Apply(loadedMode);
                TemaSeleccionado = TemasDisponibles[(int)loadedMode];
            }
            finally
            {
                _initializing = false;
            }
        }
    }
}