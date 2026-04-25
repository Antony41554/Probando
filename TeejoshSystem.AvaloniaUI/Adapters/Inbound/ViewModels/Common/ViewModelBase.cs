using CommunityToolkit.Mvvm.ComponentModel;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;
    }
}