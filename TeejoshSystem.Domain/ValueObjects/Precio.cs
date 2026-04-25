

namespace TeejoshSystem.Domain.ValueObjects
{
    public sealed class Precio
    {
        public decimal Value { get; }

        public Precio(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("El precio no puede ser negativo");

            Value = decimal.Round(value, 2);
        }

        public override bool Equals(object? obj)
            => obj is Precio other && Value == other.Value;

        public override int GetHashCode()
            => Value.GetHashCode();

        public override string ToString()
            => Value.ToString("0.00");
    }
}
