using TeejoshSystem.Domain.Entities.Detalles;
using TeejoshSystem.Domain.Enums;
using TeejoshSystem.Domain.ValueObjects;

namespace TeejoshSystem.Domain.Entities
{
    public class Producto
    {
        public int Id { get; private set; }
        public TipoProducto Tipo { get; private set; }
        public NombreProducto Nombre { get; private set; } = null!;
        public Precio Precio { get; private set; } = null!;
        public Unidades Stock { get; private set; } = null!;
        public ProductoDetalle? Descripcion { get; private set; }

        private Producto() { } // Para EF (Infrastructure)

        public Producto(
            TipoProducto tipo,
            NombreProducto nombre,
            Precio precio,
            Unidades stock)
        {
            Tipo = tipo;
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Precio = precio ?? throw new ArgumentNullException(nameof(precio));
            Stock = stock ?? throw new ArgumentNullException(nameof(stock));
        }

        public void ActualizarDatos(
            NombreProducto nombre,
            Precio precio,
            Unidades stock)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Precio = precio ?? throw new ArgumentNullException(nameof(precio));
            Stock = stock ?? throw new ArgumentNullException(nameof(stock));
        }

        // Precio
        public void CambiarPrecio(Precio nuevoPrecio)
        {
            Precio = nuevoPrecio;
        }

        // Stock
        public void AgregarStock(int cantidad)
        {
            Stock = Stock.Incrementar(cantidad);
        }

        public void ReducirStock(int cantidad)
        {
            Stock = Stock.Decrementar(cantidad);
        }

        // Descripción
        public void AsignarDescripcion(ProductoDetalle descripcion)
        {
            ArgumentNullException.ThrowIfNull(descripcion);

            var tipoEsperado = descripcion switch
            {
                HotWheelsDetalle => TipoProducto.HotWheels,
                FunkoDetalle => TipoProducto.Funko,
                TcgDetalle => TipoProducto.Tcg,
                ToyDetalle => TipoProducto.Toy,
                VariosDetalle => TipoProducto.Varios,
                _ => throw new ArgumentException("Tipo de detalle no reconocido")
            };

            if (Tipo != tipoEsperado)
                throw new InvalidOperationException(
                    $"El detalle '{descripcion.GetType().Name}' no corresponde al tipo '{Tipo}'");

            Descripcion = descripcion;
        }
    }
}
