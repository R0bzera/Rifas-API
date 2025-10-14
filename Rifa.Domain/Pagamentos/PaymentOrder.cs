namespace Rifa.Domain.Pagamentos
{
    public class PaymentOrder
    {
        public Guid Id { get; set; }
        public long PaymentId { get; set; } // ID do pagamento no Mercado Pago
        public Guid PedidoId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        
        // Navegação
        public Pedido.PedidoEntity Pedido { get; set; } = null!;
    }
}
