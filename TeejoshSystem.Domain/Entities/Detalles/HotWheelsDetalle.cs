

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public sealed class HotWheelsDetalle : ProductoDetalle
    {
        public string Modelo { get; private set; } = null!;
        public int Anio { get; private set; }
        public string Serie { get; private set; } = null!;
        public int CategoriaId { get; private set; }

        private HotWheelsDetalle() { }

        public HotWheelsDetalle(
            string modelo,
            int anio,
            string serie,
            int categoriaId)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new ArgumentException("El modelo es obligatorio");

            if (anio < 1967 || anio > DateTime.Now.Year + 1)
                throw new ArgumentException("Anio invalido");

            if (string.IsNullOrWhiteSpace(serie))
                throw new ArgumentException("La serie es obligatoria");

            Modelo = modelo;
            Anio = anio;
            Serie = serie;
            CategoriaId = categoriaId;
        }

        public void Actualizar(
            string modelo,
            int anio,
            string serie,
            int categoriaId)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new ArgumentException("El modelo es obligatorio");

            if (anio < 1967 || anio > DateTime.Now.Year + 1)
                throw new ArgumentException("Anio invalido");

            if (string.IsNullOrWhiteSpace(serie))
                throw new ArgumentException("La serie es obligatoria");

            Modelo = modelo;
            Anio = anio;
            Serie = serie;
            CategoriaId = categoriaId;
        }
    }
}
