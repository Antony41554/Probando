using CommunityToolkit.Mvvm.ComponentModel;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;

public partial class FieldValidationState : ObservableObject
{
    [ObservableProperty]
    private string? _errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public void SetError(string? message) => ErrorMessage = message;

    public void Clear() => ErrorMessage = null;

    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));
}
