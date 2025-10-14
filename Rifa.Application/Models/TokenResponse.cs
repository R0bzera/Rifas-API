namespace Rifa.Application.Models
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string ExpiresAt { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
