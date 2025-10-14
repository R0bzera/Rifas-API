using Rifa.Domain.Cota;

namespace Rifa.Application.Interfaces
{
    public interface ICotaRepository
    {
        Task<CotaEntity?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CotaEntity>> ObterTodosAsync();
        Task<IEnumerable<CotaEntity>> ObterPorRifaAsync(Guid rifaId);
        Task<IEnumerable<CotaEntity>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<CotaEntity>> ObterDisponiveisPorRifaAsync(Guid rifaId);
        Task<CotaEntity> CriarAsync(CotaEntity cota);
        Task<CotaEntity> AtualizarAsync(CotaEntity cota);
        Task ExcluirAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task ComprarCotaAsync(Guid cotaId, Guid usuarioId, Guid pedidoId);
        Task<bool> CotaDisponivelAsync(Guid cotaId);
        Task<int> ObterProximoNumeroAsync(Guid rifaId);
        Task<IEnumerable<CotaEntity>> ObterCotasAleatoriasDisponiveisAsync(Guid rifaId, int quantidade);
        Task<int> ContarCotasDisponiveisPorRifaAsync(Guid rifaId);
    }
}