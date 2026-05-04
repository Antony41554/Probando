using Avalonia.Styling;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public enum ThemeMode
{
    System = 0,
    Light = 1,
    Dark = 2
}

public interface IThemeService
{
    ThemeMode CurrentMode { get; }
    ThemeVariant CurrentThemeVariant { get; }
    void Apply(ThemeMode mode);
    ThemeVariant ToThemeVariant(ThemeMode mode);
}
