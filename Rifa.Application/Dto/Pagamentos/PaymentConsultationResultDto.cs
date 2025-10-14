namespace Rifa.Application.Dto.Pagamentos
{
    /// <summary>
    /// DTO que representa o resultado de uma consulta de pagamento com informações sobre atualizações no pedido.
    /// </summary>
    public class PaymentConsultationResultDto
    {
        /// <summary>
        /// Informações do pagamento obtidas do Mercado Pago.
        /// </summary>
        public PaymentResponseDto PaymentInfo { get; set; } = null!;

        /// <summary>
        /// Indica se o pedido foi atualizado durante a consulta.
        /// </summary>
        public bool OrderUpdated { get; set; }

        /// <summary>
        /// Mensagem descritiva sobre o resultado da operação.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
