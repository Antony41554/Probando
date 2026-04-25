using System.Threading.Tasks;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public interface INotificationService
{
    Task ShowSuccessAsync(string message);
    Task ShowErrorAsync(string message);
}