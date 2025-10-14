using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifa.Application.Dto.Pagamentos
{
    public class PaymentResponseDto
    {
        public long? Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TicketUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public DateTime? DateCreated { get; set; }
        public DateTime? DateApproved { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string? Description { get; set; }
        public string? StatusDetail { get; set; }
        
        // Informações do pedido e cotas
        public Guid? PedidoId { get; set; }
        public string RifaTitulo { get; set; } = string.Empty;
        public int QuantidadeCotas { get; set; }
        public List<int> NumerosSorte { get; set; } = new();
        public bool PagamentoConfirmado { get; set; }
    }
}
