

namespace TeejoshSystem.Domain.Entities.Detalles
{
    public sealed class FunkoDetalle : ProductoDetalle
    {
        public int NumeroCaja { get; private set; }
        public string Licencia { get; private set; } = null!;
        public int SubtipoId { get; private set; }
        public int? CaracteristicaEspecialId { get; private set; }

        private FunkoDetalle() { }

        public FunkoDetalle(
            int numeroCaja,
            string licencia,
            int subtipoId,
            int? caracteristicaEspecialId)
        {
            if (numeroCaja <= 0)
                throw new ArgumentException("Numero de caja invalido");

            if (string.IsNullOrWhiteSpace(licencia))
                throw new ArgumentException("La licencia es obligatoria");

            NumeroCaja = numeroCaja;
            Licencia = licencia;
            SubtipoId = subtipoId;
            CaracteristicaEspecialId = caracteristicaEspecialId;
        }

        public void Actualizar(
            int numeroCaja,
            string licencia,
            int subtipoId,
            int? caracteristicaEspecialId)
        {
            if (numeroCaja <= 0)
                throw new ArgumentException("Numero de caja invalido");

            if (string.IsNullOrWhiteSpace(licencia))
                throw new ArgumentException("La licencia es obligatoria");

            NumeroCaja = numeroCaja;
            Licencia = licencia;
            SubtipoId = subtipoId;
            CaracteristicaEspecialId = caracteristicaEspecialId;
        }
    }
}
