using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Behaviors;

public sealed class ValidationField
{
    public const string InvalidClass = "validationError";

    public static readonly AttachedProperty<string?> FieldNameProperty =
        AvaloniaProperty.RegisterAttached<ValidationField, InputElement, string?>("FieldName");

    public static readonly AttachedProperty<ICommand?> ValidateCommandProperty =
        AvaloniaProperty.RegisterAttached<ValidationField, InputElement, ICommand?>("ValidateCommand");

    public static readonly AttachedProperty<ICommand?> ClearCommandProperty =
        AvaloniaProperty.RegisterAttached<ValidationField, InputElement, ICommand?>("ClearCommand");

    public static readonly AttachedProperty<bool> IsInvalidProperty =
        AvaloniaProperty.RegisterAttached<ValidationField, StyledElement, bool>("IsInvalid");

    private static readonly AttachedProperty<bool> IsHookedProperty =
        AvaloniaProperty.RegisterAttached<ValidationField, InputElement, bool>("IsHooked");

    static ValidationField()
    {
        FieldNameProperty.Changed.AddClassHandler<InputElement>((element, _) => EnsureFocusHandlers(element));
        ValidateCommandProperty.Changed.AddClassHandler<InputElement>((element, _) => EnsureFocusHandlers(element));
        ClearCommandProperty.Changed.AddClassHandler<InputElement>((element, _) => EnsureFocusHandlers(element));
        IsInvalidProperty.Changed.AddClassHandler<StyledElement>((element, _) =>
            SetValidationClass(element, GetIsInvalid(element)));
    }

    public static string? GetFieldName(InputElement element) => element.GetValue(FieldNameProperty);
    public static void SetFieldName(InputElement element, string? value) => element.SetValue(FieldNameProperty, value);

    public static ICommand? GetValidateCommand(InputElement element) => element.GetValue(ValidateCommandProperty);
    public static void SetValidateCommand(InputElement element, ICommand? value) => element.SetValue(ValidateCommandProperty, value);

    public static ICommand? GetClearCommand(InputElement element) => element.GetValue(ClearCommandProperty);
    public static void SetClearCommand(InputElement element, ICommand? value) => element.SetValue(ClearCommandProperty, value);

    public static bool GetIsInvalid(StyledElement element) => element.GetValue(IsInvalidProperty);
    public static void SetIsInvalid(StyledElement element, bool value) => element.SetValue(IsInvalidProperty, value);

    private static bool GetIsHooked(InputElement element) => element.GetValue(IsHookedProperty);
    private static void SetIsHooked(InputElement element, bool value) => element.SetValue(IsHookedProperty, value);

    private static void EnsureFocusHandlers(InputElement element)
    {
        if (GetIsHooked(element))
            return;

        element.GotFocus += OnGotFocus;
        element.LostFocus += OnLostFocus;
        SetIsHooked(element, true);
    }

    private static void OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is InputElement element)
            Execute(GetClearCommand(element), GetFieldName(element));
    }

    private static void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is InputElement element)
            Execute(GetValidateCommand(element), GetFieldName(element));
    }

    private static void Execute(ICommand? command, string? fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName) || command?.CanExecute(fieldName) != true)
            return;

        command.Execute(fieldName);
    }

    private static void SetValidationClass(StyledElement element, bool isInvalid)
    {
        if (isInvalid)
        {
            if (!element.Classes.Contains(InvalidClass))
                element.Classes.Add(InvalidClass);

            return;
        }

        element.Classes.Remove(InvalidClass);
    }
}
