

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public sealed class ToyDetalle : ProductoDetalle
    {
        public int EdadMinima { get; private set; }
        public int JugadoresMin { get; private set; }
        public int JugadoresMax { get; private set; }
        public bool EsJuegoDeMesa { get; private set; }

        private ToyDetalle() { }

        public ToyDetalle(
            int edadMinima,
            int jugadoresMin,
            int jugadoresMax,
            bool esJuegoDeMesa)
        {
            if (jugadoresMax < jugadoresMin)
                throw new ArgumentException("Rango de jugadores invalido");

            EdadMinima = edadMinima;
            JugadoresMin = jugadoresMin;
            JugadoresMax = jugadoresMax;
            EsJuegoDeMesa = esJuegoDeMesa;
        }

        public void Actualizar(
            int edadMinima,
            int jugadoresMin,
            int jugadoresMax,
            bool esJuegoDeMesa)
        {
            if (jugadoresMax < jugadoresMin)
                throw new ArgumentException("Rango de jugadores invalido");

            EdadMinima = edadMinima;
            JugadoresMin = jugadoresMin;
            JugadoresMax = jugadoresMax;
            EsJuegoDeMesa = esJuegoDeMesa;
        }
    }
}
