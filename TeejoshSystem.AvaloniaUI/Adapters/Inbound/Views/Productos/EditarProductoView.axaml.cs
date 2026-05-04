using Avalonia.Controls;

using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.Views.Productos;
public partial class EditarProductoView : UserControl
{
    public EditarProductoView()
    {
        InitializeComponent();

        // Se dispara cuando la vista termina de inicializarse
        // y el DataContext ya está asignado
        DataContextChanged += (_, _) =>
        {
            if (DataContext is EditarProductoViewModel vm)
                vm.OnLoaded();
        };
    }
}
