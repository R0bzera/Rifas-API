using System.ComponentModel.DataAnnotations;

namespace Rifa.Application.Dto.Pagamentos
{
    public class PaymentRequestDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "Sobrenome deve ter no máximo 100 caracteres")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de documento é obrigatório")]
        public string DocumentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número do documento é obrigatório")]
        [StringLength(20, ErrorMessage = "Número do documento deve ter no máximo 20 caracteres")]
        public string DocumentNumber { get; set; } = string.Empty;

        // Dados específicos da rifa
        [Required(ErrorMessage = "ID da rifa é obrigatório")]
        public Guid RifaId { get; set; }

        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public Guid UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantidade de cotas deve ser maior que zero")]
        public int QuantidadeCotas { get; set; }
    }
}
