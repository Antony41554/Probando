using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common
{
    public abstract class ValidatableViewModel : ViewModelBase
    {
        private readonly Dictionary<string, List<string>> _errors = new();
        private readonly Dictionary<string, Func<string?>> _validators = new();
        private readonly Dictionary<string, FieldValidationState> _fieldStates = new();

        protected ValidatableViewModel()
        {
            ValidarCampoCommand = new RelayCommand<string?>(fieldName => ValidateField(fieldName));
            LimpiarValidacionCampoCommand = new RelayCommand<string?>(ClearFieldValidation);
        }

        public IRelayCommand<string?> ValidarCampoCommand { get; }
        public IRelayCommand<string?> LimpiarValidacionCampoCommand { get; }

        public bool HasErrors => _errors.Any();

        public event EventHandler? ErrorsChanged;

        protected FieldValidationState RegisterFieldValidation(
            string propertyName,
            Func<string?> validator)
        {
            var state = new FieldValidationState();
            _fieldStates[propertyName] = state;
            _validators[propertyName] = validator;

            return state;
        }

        protected bool ValidateField(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) ||
                !_validators.TryGetValue(propertyName, out var validator))
                return true;

            ClearErrors(propertyName);

            var error = validator();
            if (string.IsNullOrWhiteSpace(error))
            {
                SetFieldError(propertyName, null);
                return true;
            }

            AddError(propertyName, error);
            SetFieldError(propertyName, error);
            return false;
        }

        protected bool ValidateFields(IEnumerable<string> propertyNames)
        {
            var isValid = true;

            foreach (var propertyName in propertyNames)
                isValid &= ValidateField(propertyName);

            return isValid;
        }

        protected void ClearFieldValidation(string? propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return;

            ClearErrors(propertyName);
            SetFieldError(propertyName, null);
        }

        protected void ClearFieldValidations(IEnumerable<string> propertyNames)
        {
            foreach (var propertyName in propertyNames)
                ClearFieldValidation(propertyName);
        }

        private void SetFieldError(string propertyName, string? error)
        {
            if (_fieldStates.TryGetValue(propertyName, out var state))
                state.SetError(error);
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
                OnErrorsChanged(propertyName);
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, EventArgs.Empty);

            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ResumenErrores));
        }

        public interface ILoadable
        {
            void OnLoaded();
        }

        public IReadOnlyList<string> ResumenErrores =>
            _errors.Values
                .SelectMany(e => e)
                .ToList()
                .AsReadOnly();

        
    }
}
