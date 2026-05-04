using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public class ConfirmationService : IConfirmationService
{
    public async Task<bool> ConfirmAsync(string message, string title = "Confirmación")
    {
        var ventana = ObtenerVentanaPrincipal();
        if (ventana is null) return false;

        var resultado = false;

        var dialogo = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var botonSi = new Button
        {
            Content = "Sí",
            Width = 80,
            Height = 36,
            Background = SolidColorBrush.Parse("#FF9800"),
            Foreground = Brushes.White
        };

        var botonNo = new Button
        {
            Content = "No",
            Width = 80,
            Height = 36
        };

        botonSi.Click += (_, _) => { resultado = true; dialogo.Close(); };
        botonNo.Click += (_, _) => { resultado = false; dialogo.Close(); };

        dialogo.Content = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Children =
            {
                new Border
                {
                    [Grid.RowProperty] = 0,
                    Background        = SolidColorBrush.Parse("#2C3E50"),
                    Padding           = new Thickness(16, 10),
                    Child             = new TextBlock
                    {
                        Text       = title,
                        Foreground = Brushes.White,
                        FontSize   = 16,
                        FontWeight = FontWeight.SemiBold
                    }
                },
                new TextBlock
                {
                    [Grid.RowProperty] = 1,
                    Text               = message,
                    TextWrapping       = TextWrapping.Wrap,
                    Margin             = new Thickness(16),
                    VerticalAlignment  = VerticalAlignment.Center
                },
                new Border
                {
                    [Grid.RowProperty] = 2,
                    Padding            = new Thickness(16, 8),
                    Child              = new StackPanel
                    {
                        Orientation         = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing             = 8,
                        Children            = { botonSi, botonNo }
                    }
                }
            }
        };

        await dialogo.ShowDialog(ventana);
        return resultado;
    }

    private static Window? ObtenerVentanaPrincipal()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }
}