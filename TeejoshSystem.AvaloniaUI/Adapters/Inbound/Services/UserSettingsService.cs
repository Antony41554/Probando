using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public record UserSettings([property: JsonPropertyOrder(1)] ThemeMode Theme = ThemeMode.System);

public class UserSettingsService : IUserSettingsService
{
    private readonly string _settingsFilePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _lock = new();

    public UserSettingsService()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string directoryPath = Path.Combine(localAppData, "TeejoshSystem");
        _settingsFilePath = Path.Combine(directoryPath, "user-settings.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public ThemeMode LoadTheme()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                string json;
                lock (_lock)
                {
                    json = File.ReadAllText(_settingsFilePath);
                }

                var settings = JsonSerializer.Deserialize<UserSettings>(json, _jsonOptions);
                if (settings != null)
                {
                    return settings.Theme;
                }
            }
        }
        catch
        {
            // Fallback si hay error de lectura o archivo corrupto
        }

        return ThemeMode.System;
    }

    public void SaveTheme(ThemeMode mode)
    {
        try
        {
            var settings = new UserSettings(mode);
            string json = JsonSerializer.Serialize(settings, _jsonOptions);

            lock (_lock)
            {
                File.WriteAllText(_settingsFilePath, json);
            }
        }
        catch
        {
            // Ignorar errores de guardado si ocurren
        }
    }
}
