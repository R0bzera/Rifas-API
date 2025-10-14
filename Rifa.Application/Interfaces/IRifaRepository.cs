using Rifa.Domain.Rifa;

namespace Rifa.Application.Interfaces
{
    public interface IRifaRepository
    {
        Task<RifaEntity?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<RifaEntity>> ObterTodosAsync();
        Task<IEnumerable<RifaEntity>> ObterAtivasAsync();
        Task<IEnumerable<RifaEntity>> ObterFinalizadasAsync();
        Task<RifaEntity> CriarAsync(RifaEntity rifa);
        Task<RifaEntity> AtualizarAsync(RifaEntity rifa);
        Task ExcluirAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task FinalizarAsync(Guid id, Guid ganhadorId);
    }
}