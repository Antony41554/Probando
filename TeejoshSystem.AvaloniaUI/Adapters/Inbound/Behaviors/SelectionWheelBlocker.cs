using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Behaviors;

public sealed class SelectionWheelBlocker
{
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<SelectionWheelBlocker, InputElement, bool>("IsEnabled");

    static SelectionWheelBlocker()
    {
        IsEnabledProperty.Changed.AddClassHandler<InputElement>((element, args) =>
        {
            if (args.NewValue is true)
            {
                element.AddHandler(
                    InputElement.PointerWheelChangedEvent,
                    OnPointerWheelChanged,
                    RoutingStrategies.Tunnel);
            }
            else
            {
                element.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
            }
        });
    }

    public static bool GetIsEnabled(InputElement element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(InputElement element, bool value) => element.SetValue(IsEnabledProperty, value);

    private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is ComboBox { IsDropDownOpen: true })
            return;

        if (sender is not InputElement element)
            return;

        ScrollNearestParent(element, e.Delta.Y);
        e.Handled = true;
    }

    private static void ScrollNearestParent(InputElement element, double wheelDeltaY)
    {
        var scrollViewer = element.GetVisualAncestors().OfType<ScrollViewer>().FirstOrDefault();
        if (scrollViewer is null)
            return;

        var maxY = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
        var nextY = Math.Clamp(scrollViewer.Offset.Y - wheelDeltaY * 48, 0, maxY);

        scrollViewer.Offset = new Vector(scrollViewer.Offset.X, nextY);
    }
}
