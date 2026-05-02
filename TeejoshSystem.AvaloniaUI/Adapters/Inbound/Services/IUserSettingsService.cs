using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public interface IUserSettingsService
{
    ThemeMode LoadTheme();
    void SaveTheme(ThemeMode mode);
}
