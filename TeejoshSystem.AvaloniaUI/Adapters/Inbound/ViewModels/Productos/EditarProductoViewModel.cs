using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System.Threading.Tasks;

using TeejoshSystem.Application.Ports.Inbound.Productos.Commands.ActualizarProducto;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.Services;
using TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common;
using TeejoshSystem.Domain.Enums;
using static TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common.ValidatableViewModel;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Productos;

public partial class EditarProductoViewModel : ValidatableViewModel, ILoadable
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly IConfirmationService _confirmation;
    private readonly INavigationService _navigation;
    private readonly GestionarProductosViewModel _gestionarVm;
    private readonly int _productoId;
    private readonly TipoProducto _tipo;

    // ─── Propiedades del formulario ───────────────────────────────────────

    [ObservableProperty]
    private string? _nombre;

    [ObservableProperty]
    private decimal _precio;

    [ObservableProperty]
    private int _unidades;

    // ─── Propiedades de error bindeables ──────────────────────────────────

    public string? NombreError   => GetFirstError(nameof(Nombre));
    public string? PrecioError   => GetFirstError(nameof(Precio));
    public string? UnidadesError => GetFirstError(nameof(Unidades));

    // ─── Constructor ──────────────────────────────────────────────────────

    public EditarProductoViewModel(
        IMediator mediator,
        INotificationService notification,
        IConfirmationService confirmation,
        INavigationService navigation,
        GestionarProductosViewModel gestionarVm,
        int productoId,
        TipoProducto tipo)
    {
        _mediator     = mediator;
        _notification = notification;
        _confirmation = confirmation;
        _navigation   = navigation;
        _gestionarVm  = gestionarVm;
        _productoId   = productoId;
        _tipo         = tipo;

        ErrorsChanged += (_, _) => GuardarCommand.NotifyCanExecuteChanged();

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsBusy))
                GuardarCommand.NotifyCanExecuteChanged();
        };
    }

    public void OnLoaded()
    {
        // Se implementará cuando exista ObtenerProductoPorIdQuery
    }

    // ─── Comandos ─────────────────────────────────────────────────────────

    [RelayCommand]
    private void Volver() => _navigation.NavigateTo(_gestionarVm);

    [RelayCommand(CanExecute = nameof(CanGuardar))]
    private async Task GuardarAsync()
    {
        if (IsBusy) return;

        var confirmar = await _confirmation.ConfirmAsync(
            "¿Desea guardar los cambios del producto?");
        if (!confirmar) return;

        try
        {
            IsBusy = true;

            var result = await _mediator.Send(
                new ActualizarProductoCommand(
                    _productoId,
                    Nombre!,
                    Precio,
                    Unidades));

            if (result.IsSuccess)
            {
                await _notification.ShowSuccessAsync("Producto actualizado correctamente.");
                await _gestionarVm.BuscarAsync();
                _navigation.NavigateTo(_gestionarVm);
            }
            else
            {
                await _notification.ShowErrorAsync(
                    result.Error ?? "Ocurrió un error al guardar el producto.");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanGuardar() => !HasErrors && !IsBusy;

    // ─── Validaciones con notificación de error bindeable ─────────────────

    partial void OnNombreChanged(string? value)
    {
        ClearErrors(nameof(Nombre));

        if (string.IsNullOrWhiteSpace(value))
            AddError(nameof(Nombre), "El nombre es obligatorio.");
        else if (value.Length > 50)
            AddError(nameof(Nombre), "Máximo 50 caracteres.");

        OnPropertyChanged(nameof(NombreError));
    }

    partial void OnPrecioChanged(decimal value)
    {
        ClearErrors(nameof(Precio));

        if (value < 0)
            AddError(nameof(Precio), "El precio no puede ser negativo.");

        OnPropertyChanged(nameof(PrecioError));
    }

    partial void OnUnidadesChanged(int value)
    {
        ClearErrors(nameof(Unidades));

        if (value < 0)
            AddError(nameof(Unidades), "Las unidades no pueden ser negativas.");

        OnPropertyChanged(nameof(UnidadesError));
    }
}