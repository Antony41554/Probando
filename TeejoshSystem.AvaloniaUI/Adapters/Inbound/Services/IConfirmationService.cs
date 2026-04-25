using System.Threading.Tasks;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public interface IConfirmationService
{
    Task<bool> ConfirmAsync(string message, string title = "Confirmación");
}