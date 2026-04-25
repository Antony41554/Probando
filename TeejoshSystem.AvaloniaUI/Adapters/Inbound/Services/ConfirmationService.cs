using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public class ConfirmationService : IConfirmationService
{
    public async Task<bool> ConfirmAsync(string message, string title = "Confirmación")
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            title, message, ButtonEnum.YesNo, Icon.Question);
        var result = await box.ShowAsync();
        return result == ButtonResult.Yes;
    }
}