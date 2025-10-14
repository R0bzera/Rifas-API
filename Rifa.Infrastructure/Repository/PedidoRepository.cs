using Microsoft.EntityFrameworkCore;
using Rifa.Application.Interfaces;
using Rifa.Domain.Pedido;
using Rifa.Infrastructure.Config;

namespace Rifa.Infrastructure.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly RifaDbContext _context;

        public PedidoRepository(RifaDbContext context)
        {
            _context = context;
        }

        public async Task<PedidoEntity?> ObterPorIdAsync(Guid id)
        {
            try
            {
                return await _context.pedidos_rifa
                    .Include(p => p.Rifa)
                    .Include(p => p.Usuario)
                    .Include(p => p.Cotas)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar pedido: {ex.Message}");
            }
        }

        public async Task<IEnumerable<PedidoEntity>> ObterTodosAsync()
        {
            try
            {
                return await _context.pedidos_rifa
                    .Include(p => p.Rifa)
                    .Include(p => p.Usuario)
                    .Include(p => p.Cotas)
                    .AsNoTracking()
                    .OrderByDescending(p => p.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar pedidos: {ex.Message}");
            }
        }

        public async Task<IEnumerable<PedidoEntity>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            try
            {
                return await _context.pedidos_rifa
                    .Include(p => p.Rifa)
                    .Include(p => p.Usuario)
                    .Include(p => p.Cotas)
                    .AsNoTracking()
                    .Where(p => p.UsuarioId == usuarioId)
                    .OrderByDescending(p => p.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar pedidos do usuário: {ex.Message}");
            }
        }

        public async Task<IEnumerable<PedidoEntity>> ObterPorRifaAsync(Guid rifaId)
        {
            try
            {
                return await _context.pedidos_rifa
                    .Include(p => p.Rifa)
                    .Include(p => p.Usuario)
                    .Include(p => p.Cotas)
                    .AsNoTracking()
                    .Where(p => p.RifaId == rifaId)
                    .OrderByDescending(p => p.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar pedidos da rifa: {ex.Message}");
            }
        }

        public async Task<PedidoEntity> CriarAsync(PedidoEntity pedido)
        {
            try
            {
                pedido.Id = Guid.NewGuid();
                pedido.DataCriacao = DateTime.Now;
                pedido.DataAlteracao = DateTime.Now;

                await _context.pedidos_rifa.AddAsync(pedido);
                await _context.SaveChangesAsync();
                
                return pedido;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar pedido: {ex.Message}");
            }
        }

        public async Task<PedidoEntity> AtualizarAsync(PedidoEntity pedido)
        {
            try
            {
                pedido.DataAlteracao = DateTime.Now;

                _context.pedidos_rifa.Update(pedido);
                await _context.SaveChangesAsync();
                
                return pedido;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar pedido: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var pedido = await _context.pedidos_rifa.FindAsync(id);
                if (pedido != null)
                {
                    _context.pedidos_rifa.Remove(pedido);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir pedido: {ex.Message}");
            }
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            try
            {
                return await _context.pedidos_rifa.AnyAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência do pedido: {ex.Message}");
            }
        }

        public async Task ConfirmarPagamentoAsync(Guid id)
        {
            try
            {
                var pedido = await _context.pedidos_rifa.FindAsync(id);
                if (pedido != null)
                {
                    pedido.PagamentoConfirmado = true;
                    pedido.DataAlteracao = DateTime.Now;

                    _context.pedidos_rifa.Update(pedido);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao confirmar pagamento: {ex.Message}");
            }
        }
    }
}