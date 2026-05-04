using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public sealed record UserSettings([property: JsonPropertyOrder(1)] ThemeMode Theme = ThemeMode.System);

public sealed class UserSettingsService : IUserSettingsService
{
    private readonly string _settingsFilePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _lock = new();

    public UserSettingsService()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var directoryPath = Path.Combine(localAppData, "TeejoshSystem");
        _settingsFilePath = Path.Combine(directoryPath, "user-settings.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        Directory.CreateDirectory(directoryPath);
    }

    public ThemeMode LoadTheme()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
                return ThemeMode.System;

            string json;
            lock (_lock)
            {
                json = File.ReadAllText(_settingsFilePath);
            }

            return JsonSerializer.Deserialize<UserSettings>(json, _jsonOptions)?.Theme ?? ThemeMode.System;
        }
        catch
        {
            return ThemeMode.System;
        }
    }

    public void SaveTheme(ThemeMode mode)
    {
        try
        {
            var json = JsonSerializer.Serialize(new UserSettings(mode), _jsonOptions);

            lock (_lock)
            {
                File.WriteAllText(_settingsFilePath, json);
            }
        }
        catch
        {
            // Theme persistence should never block the UI workflow.
        }
    }
}
