using System.Security.Claims;

namespace Rifa.Application.Helpers
{
    public static class AuthHelper
    {
        public static Guid ObterUsuarioIdDoToken(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new Exception("Token inválido ou usuário não autenticado");

            return userId;
        }

        public static string ObterEmailDoToken(ClaimsPrincipal user)
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            
            if (string.IsNullOrEmpty(email))
                throw new Exception("Token inválido ou usuário não autenticado");

            return email;
        }

        public static string ObterNomeDoToken(ClaimsPrincipal user)
        {
            var nome = user.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(nome))
                throw new Exception("Token inválido ou usuário não autenticado");

            return nome;
        }
    }
}
