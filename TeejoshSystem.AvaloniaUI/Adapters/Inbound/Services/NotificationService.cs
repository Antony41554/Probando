using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public class NotificationService : INotificationService
{
    public async Task ShowSuccessAsync(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Éxito", message, ButtonEnum.Ok, Icon.Success);
        await box.ShowAsync();
    }

    public async Task ShowErrorAsync(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Error", message, ButtonEnum.Ok, Icon.Error);
        await box.ShowAsync();
    }
}