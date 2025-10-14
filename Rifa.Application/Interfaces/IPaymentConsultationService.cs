using Rifa.Application.Dto.Pagamentos;

namespace Rifa.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de consulta de pagamentos com atualização automática de status.
    /// </summary>
    public interface IPaymentConsultationService
    {
        /// <summary>
        /// Consulta o status do pagamento e atualiza automaticamente o pedido se o pagamento foi aprovado.
        /// </summary>
        /// <param name="paymentId">ID do pagamento no Mercado Pago</param>
        /// <returns>Informações do pagamento e indicação se houve atualização no pedido</returns>
        Task<PaymentConsultationResultDto> ConsultarPagamentoEAtualizarStatusAsync(long paymentId);
    }
}
