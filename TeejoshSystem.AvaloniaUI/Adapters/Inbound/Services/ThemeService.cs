using Avalonia;
using Avalonia.Styling;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public sealed class ThemeService : IThemeService
{
    public ThemeMode CurrentMode { get; private set; } = ThemeMode.System;

    public void Apply(ThemeMode mode)
    {
        CurrentMode = mode;

        Avalonia.Application.Current!.RequestedThemeVariant = mode switch
        {
            ThemeMode.Light => ThemeVariant.Light,
            ThemeMode.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}