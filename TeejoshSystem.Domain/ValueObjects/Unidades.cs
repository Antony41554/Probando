

namespace TeejoshSystem.Domain.ValueObjects
{
    public sealed class Unidades
    {
        public int Value { get; }

        public Unidades(int value)
        {
            if (value < 0)
                throw new ArgumentException("Las unidades no pueden ser negativas");

            Value = value;
        }

        public Unidades Incrementar(int cantidad)
            => new(Value + cantidad);

        public Unidades Decrementar(int cantidad)
        {
            if (cantidad > Value)
                throw new InvalidOperationException("Stock insuficiente");

            return new(Value - cantidad);
        }

        public override bool Equals(object? obj)
            => obj is Unidades other && Value == other.Value;

        public override int GetHashCode()
            => Value.GetHashCode();
    }
}
