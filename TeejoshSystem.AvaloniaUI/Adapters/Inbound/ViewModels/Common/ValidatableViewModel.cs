using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace TeejoshSystem.AvaloniaUI.Adapters.Inbound.ViewModels.Common
{
    public abstract class ValidatableViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            return _errors.TryGetValue(propertyName, out var errors)
                ? errors
                : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Devuelve el primer mensaje de error de una propiedad, o null si no hay errores.
        /// Uso: exponer como propiedad pública en el ViewModel concreto para binding en XAML.
        /// </summary>
        protected string? GetFirstError(string propertyName) =>
            _errors.TryGetValue(propertyName, out var list)
                ? list.FirstOrDefault()
                : null;

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
            ErrorsChanged?.Invoke(
                this,
                new DataErrorsChangedEventArgs(propertyName));

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