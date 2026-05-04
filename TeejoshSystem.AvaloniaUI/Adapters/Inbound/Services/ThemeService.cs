using Avalonia;
using Avalonia.Styling;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public sealed class ThemeService : IThemeService
{
    public ThemeMode CurrentMode { get; private set; } = ThemeMode.System;

    public ThemeVariant CurrentThemeVariant { get; private set; } = ThemeVariant.Default;

    public void Apply(ThemeMode mode)
    {
        CurrentMode = mode;
        CurrentThemeVariant = ToThemeVariant(mode);

        if (global::Avalonia.Application.Current is not null)
            global::Avalonia.Application.Current.RequestedThemeVariant = CurrentThemeVariant;
    }

    public ThemeVariant ToThemeVariant(ThemeMode mode) => mode switch
    {
        ThemeMode.Light => ThemeVariant.Light,
        ThemeMode.Dark => ThemeVariant.Dark,
        _ => ThemeVariant.Default
    };
}
