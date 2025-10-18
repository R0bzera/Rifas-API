using Rifa.Application.Dto.Rifa;

namespace Rifa.Application.Interfaces
{
    public interface IRifaService
    {
        Task<IEnumerable<RifaDTO>> ObterTodasAsync();
        Task<RifaDTO?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<RifaDTO>> ObterAtivasAsync();
        Task<RifaDTO> CriarAsync(CadastroRifaDTO dto);
        Task<RifaDTO> AtualizarAsync(Guid id, AtualizacaoRifaDTO dto);
        Task FinalizarAsync(Guid id, FinalizarRifaDTO dto);
        Task ExcluirAsync(Guid id);
        Task<SorteioResultadoDTO> SortearRifaAsync(Guid id);
        Task<NumeroSorteadoDTO> GerarNumeroSorteadoAsync(Guid id);
        Task<StatusSorteioDTO> ObterStatusSorteioAsync(Guid id);
    }
}