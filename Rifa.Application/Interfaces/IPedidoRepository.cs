using Rifa.Domain.Pedido;

namespace Rifa.Application.Interfaces
{
    public interface IPedidoRepository
    {
        Task<PedidoEntity?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<PedidoEntity>> ObterTodosAsync();
        Task<IEnumerable<PedidoEntity>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<PedidoEntity>> ObterPorRifaAsync(Guid rifaId);
        Task<PedidoEntity> CriarAsync(PedidoEntity pedido);
        Task<PedidoEntity> AtualizarAsync(PedidoEntity pedido);
        Task ExcluirAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task ConfirmarPagamentoAsync(Guid id);
    }
}