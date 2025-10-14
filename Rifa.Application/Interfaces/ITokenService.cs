using Rifa.Application.Models;
using Rifa.Domain.Usuario;

namespace Rifa.Application.Interfaces
{
    public interface ITokenService
    {
        TokenResponse GerarToken(UsuarioEntity usuario);
    }
}
