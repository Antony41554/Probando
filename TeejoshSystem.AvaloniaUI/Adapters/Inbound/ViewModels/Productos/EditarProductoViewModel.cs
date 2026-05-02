using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

using System.Globalization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
    private readonly ProductoValidationRules _validationRules = ProductoValidationRules.Default;
    private readonly int _productoId;
    private readonly TipoProducto _tipo;

    // ─── Propiedades del formulario ───────────────────────────────────────

    [ObservableProperty] private string? _nombre;
    [ObservableProperty] private string? _precioTexto = "0.00";
    [ObservableProperty] private string? _unidadesTexto = "0";

    // ─── Estado visual de validación ──────────────────────────────────────

    public FieldValidationState NombreValidation { get; }
    public FieldValidationState PrecioValidation { get; }
    public FieldValidationState UnidadesValidation { get; }

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
        _mediator = mediator;
        _notification = notification;
        _confirmation = confirmation;
        _navigation = navigation;
        _gestionarVm = gestionarVm;
        _productoId = productoId;
        _tipo = tipo;

        NombreValidation = RegisterFieldValidation(nameof(Nombre), ValidateNombre);
        PrecioValidation = RegisterFieldValidation(nameof(PrecioTexto), ValidatePrecio);
        UnidadesValidation = RegisterFieldValidation(nameof(UnidadesTexto), ValidateUnidades);

        ErrorsChanged += (_, _) => GuardarCommand.NotifyCanExecuteChanged();
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsBusy))
                GuardarCommand.NotifyCanExecuteChanged();
        };
    }

    public void OnLoaded()
    {
        // Pendiente: implementar cuando exista ObtenerProductoPorIdQuery
    }

    // ─── Validación: limpiar al entrar/escribir, validar al salir ─────────

    private bool ValidarFormulario() => ValidateFields(new[]
    {
        nameof(Nombre),
        nameof(PrecioTexto),
        nameof(UnidadesTexto)
    });

    private string? ValidateNombre()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
            return "*Nombre inválido";

        return Nombre.Trim().Length > _validationRules.NombreMaxLength
            ? $"*El nombre no puede superar los {_validationRules.NombreMaxLength} caracteres"
            : null;
    }

    private string? ValidatePrecio()
    {
        if (string.IsNullOrWhiteSpace(PrecioTexto) ||
            !Regex.IsMatch(PrecioTexto.Trim(), _validationRules.PrecioFormatoPattern, RegexOptions.CultureInvariant))
            return _validationRules.PrecioFormatoMensaje;

        return ParsePrecio() < _validationRules.PrecioMinimo
            ? "*Precio inválido"
            : null;
    }

    private decimal ParsePrecio() => decimal.Parse(
        PrecioTexto!.Trim(),
        NumberStyles.AllowDecimalPoint,
        CultureInfo.InvariantCulture);

    private string? ValidateUnidades() => !TryParseInt(UnidadesTexto, out var unidades) ||
                                          unidades < _validationRules.UnidadesMinimas
        ? "*Unidades inválidas"
        : null;

    private static bool TryParseInt(string? value, out int result) =>
        int.TryParse(value?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out result);

    private static int ParseInt(string? value) =>
        int.Parse(value!.Trim(), NumberStyles.None, CultureInfo.InvariantCulture);

    partial void OnNombreChanged(string? value) => ClearFieldValidation(nameof(Nombre));
    partial void OnPrecioTextoChanged(string? value) => ClearFieldValidation(nameof(PrecioTexto));
    partial void OnUnidadesTextoChanged(string? value) => ClearFieldValidation(nameof(UnidadesTexto));

    // ─── Comandos ─────────────────────────────────────────────────────────

    [RelayCommand]
    private void Volver() => _navigation.NavigateTo(_gestionarVm);

    [RelayCommand(CanExecute = nameof(CanGuardar))]
    private async Task GuardarAsync()
    {
        if (IsBusy) return;

        if (!ValidarFormulario())
            return;

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
                    ParsePrecio(),
                    ParseInt(UnidadesTexto)));

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
}
