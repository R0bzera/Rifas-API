using Microsoft.EntityFrameworkCore;
using Rifa.Application.Interfaces;
using Rifa.Domain.Usuario;
using Rifa.Infrastructure.Config;

namespace Rifa.Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly RifaDbContext _context;

        public UsuarioRepository(RifaDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioEntity?> ObterPorIdAsync(Guid id)
        {
            try
            {
                return await _context.usuarios_rifa
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar usuário por ID: {ex.Message}", ex);
            }
        }

        public async Task<UsuarioEntity?> ObterPorEmailAsync(string email)
        {
            try
            {
                return await _context.usuarios_rifa
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar usuário por email: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<UsuarioEntity>> ObterTodosAsync()
        {
            try
            {
                return await _context.usuarios_rifa
                    .AsNoTracking()
                    .OrderBy(u => u.Nome)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar todos os usuários: {ex.Message}", ex);
            }
        }

        public async Task<UsuarioEntity> CriarAsync(UsuarioEntity usuario)
        {
            try
            {
                usuario.Id = Guid.NewGuid();
                usuario.DataCriacao = DateTime.Now;
                usuario.DataAlteracao = DateTime.Now;

                await _context.usuarios_rifa.AddAsync(usuario);
                await _context.SaveChangesAsync();
                
                return usuario;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Erro ao criar usuário no banco de dados: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado ao criar usuário: {ex.Message}", ex);
            }
        }

        public async Task<UsuarioEntity> AtualizarAsync(UsuarioEntity usuario)
        {
            try
            {
                usuario.DataAlteracao = DateTime.Now;

                _context.usuarios_rifa.Update(usuario);
                await _context.SaveChangesAsync();
                
                return usuario;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception($"Erro de concorrência ao atualizar usuário: {ex.Message}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Erro ao atualizar usuário no banco de dados: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado ao atualizar usuário: {ex.Message}", ex);
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var usuario = await _context.usuarios_rifa.FindAsync(id);
                if (usuario != null)
                {
                    _context.usuarios_rifa.Remove(usuario);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Erro ao excluir usuário do banco de dados: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado ao excluir usuário: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExisteAsync(Guid id)
        {
            try
            {
                return await _context.usuarios_rifa
                    .AnyAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência de usuário: {ex.Message}", ex);
            }
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            try
            {
                return await _context.usuarios_rifa
                    .AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência de email: {ex.Message}", ex);
            }
        }
    }
}