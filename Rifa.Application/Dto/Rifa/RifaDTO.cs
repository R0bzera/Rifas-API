using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Rifa.Application.Dto.Rifa
{
    public class CadastroRifaDTO
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        public string? Imagem { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Número de cotas é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Número de cotas deve ser maior que zero")]
        public int NumCotas { get; set; }
    }

    public class RifaDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? Imagem { get; set; }
        public decimal Preco { get; set; }
        public int NumCotas { get; set; }
        public int CotasDisponiveis { get; set; }
        public Guid? GanhadorId { get; set; }
        public string? GanhadorNome { get; set; }
        public bool Finalizada { get; set; }
        public string DataCriacao { get; set; } = string.Empty;
        public string DataAlteracao { get; set; } = string.Empty;
    }

    public class AtualizacaoRifaDTO
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        public string? Imagem { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Número de cotas é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Número de cotas deve ser maior que zero")]
        public int NumCotas { get; set; }
    }

    public class FinalizarRifaDTO
    {
        [Required(ErrorMessage = "ID do ganhador é obrigatório")]
        public Guid GanhadorId { get; set; }
    }

    public class CriarRifaRequest
    {
        [Required]
        public string Titulo { get; set; } = null!;

        public string? Descricao { get; set; }

        [Required]
        [Range(typeof(decimal), "0,01", "9999999999")]
        public decimal Preco { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumCotas { get; set; }

        // IFormFile dentro do DTO — é isso que você perguntou.
        public IFormFile? Imagem { get; set; }
    }
}
