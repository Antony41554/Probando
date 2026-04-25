using TeejoshSystem.Domain.Entities.Catalogos;

namespace TeejoshSystem.Domain.Ports.Outbound.Repositories
{
    public interface ICatalogoRepository
    {
        Task<List<HotWheelsCategoria>> GetHotWheelsCategoriasAsync();
        Task<List<FunkoSubtipo>> GetFunkoSubtiposAsync();
        Task<List<FunkoCaracteristica>> GetFunkoCaracteristicasAsync();
        Task<List<TcgFranquicia>> GetTcgFranquiciasAsync();
        Task<List<TcgExpansion>> GetTcgExpansionesAsync(int franquiciaId);
        Task<List<TcgPack>> GetTcgPacksAsync(int franquiciaId);
    }
}