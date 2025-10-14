using Rifa.Application.Dto.Pedido;

namespace Rifa.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDTO>> ObterTodosAsync();
        Task<PedidoDTO?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<PedidoDTO>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<PedidoDTO> CriarAsync(CadastroPedidoDTO dto);
        Task ConfirmarPagamentoAsync(Guid id);
        Task ExcluirAsync(Guid id);
    }
}