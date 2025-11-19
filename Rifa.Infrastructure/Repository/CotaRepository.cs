using Microsoft.EntityFrameworkCore;
using Rifa.Application.Interfaces;
using Rifa.Domain.Cota;
using Rifa.Infrastructure.Config;

namespace Rifa.Infrastructure.Repository
{
    public class CotaRepository : ICotaRepository
    {
        private readonly RifaDbContext _context;

        public CotaRepository(RifaDbContext context)
        {
            _context = context;
        }

        public async Task<CotaEntity?> ObterPorIdAsync(Guid id)
        {
            try
            {
                return await _context.cotas_rifa
                    .Include(c => c.Rifa)
                    .Include(c => c.Usuario)
                    .Include(c => c.Pedido)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar cota: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaEntity>> ObterTodosAsync()
        {
            try
            {
                return await _context.cotas_rifa
                    .Include(c => c.Rifa)
                    .Include(c => c.Usuario)
                    .Include(c => c.Pedido)
                    .AsNoTracking()
                    .OrderBy(c => c.RifaId)
                    .ThenBy(c => c.Numero)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar cotas: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaEntity>> ObterPorRifaAsync(Guid rifaId)
        {
            try
            {
                return await _context.cotas_rifa
                    .Include(c => c.Rifa)
                    .Include(c => c.Usuario)
                    .Include(c => c.Pedido)
                    .AsNoTracking()
                    .Where(c => c.RifaId == rifaId)
                    .OrderBy(c => c.Numero)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar cotas da rifa: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaEntity>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            try
            {
                return await _context.cotas_rifa
                    .Include(c => c.Rifa)
                    .Include(c => c.Usuario)
                    .Include(c => c.Pedido)
                    .AsNoTracking()
                    .Where(c => c.UsuarioId == usuarioId)
                    .OrderBy(c => c.RifaId)
                    .ThenBy(c => c.Numero)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar cotas do usuário: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaEntity>> ObterDisponiveisPorRifaAsync(Guid rifaId)
        {
            try
            {
                return await _context.cotas_rifa
                    .Include(c => c.Rifa)
                    .AsNoTracking()
                    .Where(c => c.RifaId == rifaId && c.UsuarioId == null)
                    .OrderBy(c => c.Numero)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar cotas disponíveis: {ex.Message}");
            }
        }

        public async Task<CotaEntity> CriarAsync(CotaEntity cota)
        {
            try
            {
                cota.Id = Guid.NewGuid();
                cota.DataCriacao = DateTime.Now;
                cota.DataAlteracao = DateTime.Now;

                await _context.cotas_rifa.AddAsync(cota);
                await _context.SaveChangesAsync();
                
                return cota;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar cota: {ex.Message}");
            }
        }

        public async Task<CotaEntity> AtualizarAsync(CotaEntity cota)
        {
            try
            {
                cota.DataAlteracao = DateTime.Now;

                _context.cotas_rifa.Update(cota);
                await _context.SaveChangesAsync();
                
                return cota;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar cota: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var cota = await _context.cotas_rifa.FindAsync(id);
                if (cota != null)
                {
                    _context.cotas_rifa.Remove(cota);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir cota: {ex.Message}");
            }
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            try
            {
                return await _context.cotas_rifa.AnyAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência da cota: {ex.Message}");
            }
        }

        public async Task ComprarCotaAsync(Guid cotaId, Guid usuarioId, Guid pedidoId)
        {
            try
            {
                var cota = await _context.cotas_rifa.FindAsync(cotaId);
                if (cota != null)
                {
                    cota.UsuarioId = usuarioId;
                    cota.PedidoId = pedidoId == Guid.Empty ? null : pedidoId;
                    cota.DataAlteracao = DateTime.Now;

                    _context.cotas_rifa.Update(cota);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Cota com ID {cotaId} não encontrada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao comprar cota: {ex.Message}");
            }
        }

        public async Task<bool> CotaDisponivelAsync(Guid cotaId)
        {
            try
            {
                return await _context.cotas_rifa
                    .AnyAsync(c => c.Id == cotaId && c.UsuarioId == null);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar disponibilidade da cota: {ex.Message}");
            }
        }

        public async Task<int> ObterProximoNumeroAsync(Guid rifaId)
        {
            try
            {
                var ultimoNumero = await _context.cotas_rifa
                    .Where(c => c.RifaId == rifaId)
                    .MaxAsync(c => (int?)c.Numero);

                return (ultimoNumero ?? 0) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter próximo número de cota: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaEntity>> ObterCotasAleatoriasDisponiveisAsync(Guid rifaId, int quantidade)
        {
            try
            {
                var todasCotasDisponiveis = await _context.cotas_rifa
                    .Where(c => c.RifaId == rifaId && c.UsuarioId == null)
                    .ToListAsync();

                var random = new Random();
                var cotasEmbaralhadas = todasCotasDisponiveis.OrderBy(x => random.Next()).Take(quantidade);

                return cotasEmbaralhadas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter cotas aleatórias disponíveis: {ex.Message}");
            }
        }

        public async Task<int> ContarCotasDisponiveisPorRifaAsync(Guid rifaId)
        {
            try
            {
                return await _context.cotas_rifa
                    .Where(c => c.RifaId == rifaId && c.UsuarioId == null)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao contar cotas disponíveis: {ex.Message}");
            }
        }
    }
}