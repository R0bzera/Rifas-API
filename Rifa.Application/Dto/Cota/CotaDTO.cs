using System.ComponentModel.DataAnnotations;

namespace Rifa.Application.Dto.Cota
{
    public class CadastroCotaDTO
    {
        [Required(ErrorMessage = "Número da cota é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Número da cota deve ser maior que zero")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "ID da rifa é obrigatório")]
        public Guid RifaId { get; set; }
    }

    public class CotaDTO
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Guid RifaId { get; set; }
        public string RifaTitulo { get; set; } = string.Empty;
        public Guid? UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public Guid? PedidoId { get; set; }
        public string DataCriacao { get; set; } = string.Empty;
        public string DataAlteracao { get; set; } = string.Empty;
    }

    public class ComprarCotaDTO
    {
        [Required(ErrorMessage = "ID da cota é obrigatório")]
        public Guid CotaId { get; set; }

        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public Guid UsuarioId { get; set; }
    }

    public class CotaDisponivelDTO
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Guid RifaId { get; set; }
        public string RifaTitulo { get; set; } = string.Empty;
        public decimal Preco { get; set; }
    }
}
