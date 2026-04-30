// Ruta: TeejoshSystem.AvaloniaUI/Adapters/Inbound/Behaviors/ValidateOnLostFocusBehavior.cs

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Behaviors;

/// <summary>
/// Oculta los mensajes de error de un campo hasta que el usuario
/// lo haya visitado al menos una vez (perdido el foco).
/// Mejora UX evitando errores prematuros en formularios vacíos.
/// </summary>
public class ValidateOnLostFocusBehavior : Behavior<Control>
{
    public static readonly StyledProperty<bool> IsVisitedProperty =
        AvaloniaProperty.Register<ValidateOnLostFocusBehavior, bool>(nameof(IsVisited));

    public bool IsVisited
    {
        get => GetValue(IsVisitedProperty);
        private set => SetValue(IsVisitedProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.LostFocus += OnLostFocus;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject!.LostFocus -= OnLostFocus;
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
        => IsVisited = true;
}