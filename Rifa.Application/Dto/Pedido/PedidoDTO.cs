using System.ComponentModel.DataAnnotations;

namespace Rifa.Application.Dto.Pedido
{
    public class CadastroPedidoDTO
    {
        [Required(ErrorMessage = "ID da rifa é obrigatório")]
        public Guid RifaId { get; set; }

        [Required(ErrorMessage = "Total de cotas é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Total de cotas deve ser maior que zero")]
        public int TotalCotas { get; set; }

        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public Guid UsuarioId { get; set; }
    }

    public class PedidoDTO
    {
        public Guid Id { get; set; }
        public Guid RifaId { get; set; }
        public string RifaTitulo { get; set; } = string.Empty;
        public string? RifaImagem { get; set; }
        public int TotalCotas { get; set; }
        public decimal? PrecoUnitario { get; set; }
        public decimal? ValorTotal { get; set; }
        public bool PagamentoConfirmado { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string DataCriacao { get; set; } = string.Empty;
        public string DataAlteracao { get; set; } = string.Empty;
        public List<Cota.CotaDTO> Cotas { get; set; } = new();
        
        // Campos para compatibilidade com frontend
        public string Status => PagamentoConfirmado ? "confirmed" : "pending";
        public string StatusPagamento => Status;
        public string PurchaseDate => DataCriacao;
        public string DataCompra => DataCriacao;
        public int Quantity => TotalCotas;
        public int Quantidade => TotalCotas;
        public decimal? Price => PrecoUnitario;
        public decimal? ValorUnitario => PrecoUnitario;
        public string PaymentMethod => "pix";
        public string MetodoPagamento => "pix";
        public string? ImagemRifa => RifaImagem;
        public string? TituloRifa => RifaTitulo;
    }

    public class ConfirmarPagamentoPedidoDTO
    {
        [Required(ErrorMessage = "ID do pedido é obrigatório")]
        public Guid PedidoId { get; set; }
    }
}
