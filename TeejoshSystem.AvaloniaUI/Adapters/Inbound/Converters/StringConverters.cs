using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TeejoshSystem.AvaloniaUI.Converters
{
    public static class StringConverters
    {
        public static readonly IValueConverter IsNotNullOrEmpty = 
            new FuncValueConverter<string?, bool>(value =>
                !string.IsNullOrWhiteSpace(value));
    }
}