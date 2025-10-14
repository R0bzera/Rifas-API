using Microsoft.EntityFrameworkCore;
using Rifa.Application.Interfaces;
using Rifa.Domain.Rifa;
using Rifa.Infrastructure.Config;

namespace Rifa.Infrastructure.Repository
{
    public class RifaRepository : IRifaRepository
    {
        private readonly RifaDbContext _context;

        public RifaRepository(RifaDbContext context)
        {
            _context = context;
        }

        public async Task<RifaEntity?> ObterPorIdAsync(Guid id)
        {
            try
            {
                return await _context.rifas
                    .Include(r => r.Ganhador)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar rifa: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RifaEntity>> ObterTodosAsync()
        {
            try
            {
                return await _context.rifas
                    .Include(r => r.Ganhador)
                    .AsNoTracking()
                    .OrderByDescending(r => r.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar rifas: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RifaEntity>> ObterAtivasAsync()
        {
            try
            {
                return await _context.rifas
                    .Include(r => r.Ganhador)
                    .AsNoTracking()
                    .Where(r => !r.Finalizada)
                    .OrderByDescending(r => r.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar rifas ativas: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RifaEntity>> ObterFinalizadasAsync()
        {
            try
            {
                return await _context.rifas
                    .Include(r => r.Ganhador)
                    .AsNoTracking()
                    .Where(r => r.Finalizada)
                    .OrderByDescending(r => r.DataCriacao)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar rifas finalizadas: {ex.Message}");
            }
        }

        public async Task<RifaEntity> CriarAsync(RifaEntity rifa)
        {
            try
            {
                rifa.Id = Guid.NewGuid();
                rifa.DataCriacao = DateTime.Now;
                rifa.DataAlteracao = DateTime.Now;

                await _context.rifas.AddAsync(rifa);
                await _context.SaveChangesAsync();
                
                return rifa;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar rifa: {ex.Message}");
            }
        }

        public async Task<RifaEntity> AtualizarAsync(RifaEntity rifa)
        {
            try
            {
                rifa.DataAlteracao = DateTime.Now;

                _context.rifas.Update(rifa);
                await _context.SaveChangesAsync();
                
                return rifa;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar rifa: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var rifa = await _context.rifas.FindAsync(id);
                if (rifa != null)
                {
                    _context.rifas.Remove(rifa);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir rifa: {ex.Message}");
            }
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            try
            {
                return await _context.rifas.AnyAsync(r => r.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência da rifa: {ex.Message}");
            }
        }

        public async Task FinalizarAsync(Guid id, Guid ganhadorId)
        {
            try
            {
                var rifa = await _context.rifas.FindAsync(id);
                if (rifa != null)
                {
                    rifa.Finalizada = true;
                    rifa.GanhadorId = ganhadorId;
                    rifa.DataAlteracao = DateTime.Now;

                    _context.rifas.Update(rifa);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao finalizar rifa: {ex.Message}");
            }
        }
    }
}