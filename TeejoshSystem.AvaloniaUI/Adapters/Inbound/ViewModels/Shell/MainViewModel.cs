using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Styling;
using System;
using System.Collections.ObjectModel;

using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Shell
{
    public sealed record TemaItem(string Nombre, ThemeMode Modo);

    public partial class MainViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IThemeService _themeService;
        private readonly IUserSettingsService _userSettingsService;
        private bool _initializingTheme;

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private ThemeVariant _themeVariant = ThemeVariant.Default;

        [ObservableProperty]
        private TemaItem? _temaSeleccionado;

        public ObservableCollection<TemaItem> TemasDisponibles { get; } = new()
        {
            new TemaItem("💻 Sistema", ThemeMode.System),
            new TemaItem("☀️ Claro", ThemeMode.Light),
            new TemaItem("🌙 Oscuro", ThemeMode.Dark)
        };

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
            var loadedMode = _userSettingsService.LoadTheme();
            ApplyTheme(loadedMode, persist: false);

            _initializingTheme = true;
            try
            {
                TemaSeleccionado = GetTemaItem(loadedMode);
            }
            finally
            {
                _initializingTheme = false;
            }
        }

        partial void OnTemaSeleccionadoChanged(TemaItem? value)
        {
            if (value is null || _initializingTheme)
                return;

            ApplyTheme(value.Modo, persist: true);
        }

        private void ApplyTheme(ThemeMode mode, bool persist)
        {
            _themeService.Apply(mode);
            ThemeVariant = _themeService.CurrentThemeVariant;

            if (persist)
                _userSettingsService.SaveTheme(mode);
        }

        private TemaItem GetTemaItem(ThemeMode mode)
        {
            foreach (var item in TemasDisponibles)
            {
                if (item.Modo == mode)
                    return item;
            }

            return TemasDisponibles[0];
        }
    }
}
