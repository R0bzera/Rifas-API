using Rifa.Application.Dto.Usuario;
using Rifa.Application.Models;

namespace Rifa.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDTO>> ObterTodosAsync();
        Task<UsuarioDTO?> ObterPorIdAsync(Guid id);
        Task<UsuarioDTO> CriarAsync(CadastroUsuarioDTO dto);
        Task<UsuarioDTO> AtualizarAsync(Guid id, AtualizacaoUsuarioDTO dto);
        Task ExcluirAsync(Guid id);
        Task<TokenResponse> LoginAsync(LoginUsuarioDTO dto);
        Task AlterarSenhaAsync(Guid id, AlterarSenhaDTO dto);
        Task AlterarRoleAsync(Guid id, AlterarRoleDTO dto);
    }
}
