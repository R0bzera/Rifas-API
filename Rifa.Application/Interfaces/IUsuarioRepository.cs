using Rifa.Domain.Usuario;

namespace Rifa.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioEntity?> ObterPorIdAsync(Guid id);
        Task<UsuarioEntity?> ObterPorEmailAsync(string email);
        Task<IEnumerable<UsuarioEntity>> ObterTodosAsync();
        Task<UsuarioEntity> CriarAsync(UsuarioEntity usuario);
        Task<UsuarioEntity> AtualizarAsync(UsuarioEntity usuario);
        Task ExcluirAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task<bool> EmailExisteAsync(string email);
    }
}
