using Avalonia.Data.Converters;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Converters;

public static class StringConverters
{
    public static readonly IValueConverter IsNotNullOrEmpty =
        new FuncValueConverter<string?, bool>(value =>
            !string.IsNullOrWhiteSpace(value));
}
