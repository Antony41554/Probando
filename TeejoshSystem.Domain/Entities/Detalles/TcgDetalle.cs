

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public sealed class TcgDetalle : ProductoDetalle
    {
        public int PackId { get; private set; }
        public int ExpansionId { get; private set; }

        private TcgDetalle() { }

        public TcgDetalle(
            int packId,
            int expansionId)
        {
            PackId = packId;
            ExpansionId = expansionId;
        }

        public void Actualizar(
            int packId,
            int expansionId)
        {
            PackId = packId;
            ExpansionId = expansionId;
        }
    }
}
