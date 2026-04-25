

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public abstract class ProductoDetalle
    {
        public int ProductoId { get; set; }

        protected ProductoDetalle() { }

        // Método público para asignar ProductoId
        public void AsignarProductoId(int productoId)
        {
            if (productoId <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor a 0");

            ProductoId = productoId;
        }
    }
}
