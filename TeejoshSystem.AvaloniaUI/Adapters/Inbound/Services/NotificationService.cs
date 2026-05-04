using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public class NotificationService : INotificationService
{
    public Task ShowSuccessAsync(string message)
        => MostrarDialogoAsync("Éxito", message, "#43A047");

    public Task ShowErrorAsync(string message)
        => MostrarDialogoAsync("Error", message, "#E53935");

    private static async Task MostrarDialogoAsync(
    string titulo, string mensaje, string colorHeader)
    {
        var ventana = ObtenerVentanaPrincipal();
        if (ventana is null) return;

        var botonAceptar = new Button
        {
            Content = "Aceptar",
            Width = 90,
            Height = 36,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var botonCopiar = new Button
        {
            Content = "Copiar",
            Width = 90,
            Height = 36,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var dialogo = new Window
        {
            Title = titulo,
            Width = 400,
            Height = 200,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        botonAceptar.Click += (_, _) => dialogo.Close();
        botonCopiar.Click += async (_, _) =>
        {
            var clipboard = ventana.Clipboard;
            if (clipboard is not null)
                await clipboard.SetTextAsync(mensaje);
        };

        dialogo.Content = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Children =
        {
            new Border
            {
                [Grid.RowProperty] = 0,
                Background        = SolidColorBrush.Parse(colorHeader),
                Padding           = new Thickness(16, 10),
                Child             = new TextBlock
                {
                    Text       = titulo,
                    Foreground = Brushes.White,
                    FontSize   = 16,
                    FontWeight = FontWeight.SemiBold
                }
            },
            new TextBlock
            {
                [Grid.RowProperty] = 1,
                Text               = mensaje,
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
                    Children            = { botonCopiar, botonAceptar }
                }
            }
        }
        };

        await dialogo.ShowDialog(ventana);
    }

    private static Window? ObtenerVentanaPrincipal()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }
}