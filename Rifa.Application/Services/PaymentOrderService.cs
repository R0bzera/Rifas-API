using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Rifa.Application.Services
{
    public class PaymentOrderService : IPaymentOrderService
    {
        private readonly IPaymentService _paymentService;
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PaymentOrderService> _logger;

        public PaymentOrderService(
            IPaymentService paymentService,
            IPedidoService pedidoService,
            ILogger<PaymentOrderService> logger)
        {
            _paymentService = paymentService;
            _pedidoService = pedidoService;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> GetPaymentWithOrderInfoAsync(long paymentId)
        {
            try
            {
                _logger.LogInformation("Buscando informações do pagamento com pedido. PaymentId: {PaymentId}", paymentId);

                // Buscar status do pagamento
                var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentId);
                
                // TODO: Implementar busca do pedido pelo paymentId
                // Por enquanto, retornamos apenas as informações do pagamento
                // Em uma implementação completa, você salvaria o paymentId no pedido
                
                return paymentStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar informações do pagamento com pedido. PaymentId: {PaymentId}", paymentId);
                throw;
            }
        }

        public async Task ConfirmPaymentAndOrderAsync(long paymentId)
        {
            try
            {
                _logger.LogInformation("Confirmando pagamento e pedido. PaymentId: {PaymentId}", paymentId);

                // Buscar status do pagamento
                var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentId);
                
                if (paymentStatus.Status?.ToLower() == "approved")
                {
                    // TODO: Implementar confirmação do pedido pelo paymentId
                    // Em uma implementação completa, você:
                    // 1. Buscaria o pedido pelo paymentId
                    // 2. Confirmaria o pagamento no pedido
                    // 3. Atualizaria as cotas como definitivamente vendidas
                    
                    _logger.LogInformation("Pagamento aprovado. PaymentId: {PaymentId}", paymentId);
                }
                else
                {
                    _logger.LogInformation("Pagamento não aprovado. PaymentId: {PaymentId}, Status: {Status}", 
                        paymentId, paymentStatus.Status);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar pagamento e pedido. PaymentId: {PaymentId}", paymentId);
                throw;
            }
        }
    }
}
