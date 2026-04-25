

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public sealed class VariosDetalle : ProductoDetalle
    {
        public string Marca { get; private set; } = null!;
        public decimal Alto { get; private set; }
        public decimal Ancho { get; private set; }
        public decimal? Largo { get; private set; }
        public string Material { get; private set; } = null!;
        public bool TieneIlustracion { get; private set; }

        private VariosDetalle() { }

        public VariosDetalle(
            string marca,
            decimal alto,
            decimal ancho,
            decimal? largo,
            string material,
            bool tieneIlustracion)
        {
            if (alto <= 0 || ancho <= 0)
                throw new ArgumentException("Dimensiones invalidas");

            Marca = marca;
            Alto = alto;
            Ancho = ancho;
            Largo = largo;
            Material = material;
            TieneIlustracion = tieneIlustracion;
        }

        public void Actualizar(
            string marca,
            decimal alto,
            decimal ancho,
            decimal? largo,
            string material,
            bool tieneIlustracion)
        {
            if (alto <= 0 || ancho <= 0)
                throw new ArgumentException("Dimensiones invalidas");

            Marca = marca;
            Alto = alto;
            Ancho = ancho;
            Largo = largo;
            Material = material;
            TieneIlustracion = tieneIlustracion;
        }
    }
}
