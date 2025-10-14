using Rifa.Application.Dto.Cota;

namespace Rifa.Application.Interfaces
{
    public interface ICotaService
    {
        Task<IEnumerable<CotaDTO>> ObterTodasAsync();
        Task<CotaDTO?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CotaDTO>> ObterPorRifaAsync(Guid rifaId);
        Task<IEnumerable<CotaDTO>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<CotaDisponivelDTO>> ObterDisponiveisPorRifaAsync(Guid rifaId);
        Task<CotaDTO> CriarAsync(CadastroCotaDTO dto);
        Task ComprarCotaAsync(ComprarCotaDTO dto);
        Task ExcluirAsync(Guid id);
    }
}