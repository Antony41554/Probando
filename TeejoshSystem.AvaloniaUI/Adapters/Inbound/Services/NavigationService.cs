using System;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;

public class NavigationService : INavigationService
{
    private Action<object>? _navigate;
    private Action? _navigateToMenu;

    public void Configure(Action<object> navigate, Action navigateToMenu)
    {
        _navigate = navigate;
        _navigateToMenu = navigateToMenu;
    }

    public void NavigateTo(object viewModel) => _navigate?.Invoke(viewModel);
    public void NavigateToMenu() => _navigateToMenu?.Invoke();
}