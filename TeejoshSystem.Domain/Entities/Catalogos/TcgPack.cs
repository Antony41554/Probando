

namespace TeejoshSystem.Domain.Entities.Catalogos
{
    public class TcgPack
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int FranquiciaId { get; set; }
    }
}
