

namespace TeejoshSystem.Domain.ValueObjects
{
    public sealed class NombreProducto
    {
        public string Value { get; }

        public NombreProducto(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del producto es obligatorio");

            if (value.Length > 100)
                throw new ArgumentException("El nombre no puede exceder 100 caracteres");

            Value = value.Trim();
        }

        public override bool Equals(object? obj)
            => obj is NombreProducto other && Value == other.Value;

        public override int GetHashCode()
            => Value.GetHashCode();

        public override string ToString()
            => Value;
    }
}
