using Microsoft.Extensions.Logging;
using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;
using Rifa.Domain.Pagamentos;

namespace Rifa.Application.Services
{
    public class PaymentStatusService : IPaymentStatusService
    {
        private readonly IPaymentService _paymentService;
        private readonly IPedidoService _pedidoService;
        private readonly IPaymentOrderRepository _paymentOrderRepository;
        private readonly ILogger<PaymentStatusService> _logger;

        public PaymentStatusService(
            IPaymentService paymentService, 
            IPedidoService pedidoService,
            IPaymentOrderRepository paymentOrderRepository,
            ILogger<PaymentStatusService> logger)
        {
            _paymentService = paymentService;
            _pedidoService = pedidoService;
            _paymentOrderRepository = paymentOrderRepository;
            _logger = logger;
        }

        public async Task ProcessWebhookNotificationAsync(WebhookNotificationDto notification)
        {
            try
            {
                if (notification?.Data?.Id == null)
                {
                    _logger.LogWarning("Webhook notification received without payment ID");
                    return;
                }

                if (!long.TryParse(notification.Data.Id, out var paymentId))
                {
                    _logger.LogWarning("Invalid payment ID in webhook notification: {PaymentId}", notification.Data.Id);
                    return;
                }

                _logger.LogInformation("Processing webhook notification for payment {PaymentId}, action: {Action}", 
                    paymentId, notification.Action);

                // Busca o status atual do pagamento
                var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentId);
                
                // Atualiza o status no sistema
                await UpdatePaymentStatusAsync(paymentId, paymentStatus.Status);

                _logger.LogInformation("Payment {PaymentId} status updated to: {Status}", paymentId, paymentStatus.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook notification for payment {PaymentId}", 
                    notification?.Data?.Id);
                throw;
            }
        }

        public async Task UpdatePaymentStatusAsync(long paymentId, string status)
        {
            try
            {
                _logger.LogInformation("Updating payment {PaymentId} status to: {Status}", paymentId, status);

                // Buscar a relação entre pagamento e pedido
                var paymentOrder = await _paymentOrderRepository.ObterPorPaymentIdAsync(paymentId);

                if (paymentOrder != null)
                {
                    _logger.LogInformation("Encontrada relação Payment-Order. PedidoId: {PedidoId}", paymentOrder.PedidoId);

                    // Atualizar status do pagamento
                    paymentOrder.Status = status;
                    paymentOrder.DataAlteracao = DateTime.Now;

                    // Se pagamento foi aprovado, confirmar o pedido
                    if (status.ToLower() == "approved")
                    {
                        _logger.LogInformation("Pagamento aprovado. Confirmando pedido {PedidoId}", paymentOrder.PedidoId);
                        await _pedidoService.ConfirmarPagamentoAsync(paymentOrder.PedidoId);
                        
                        _logger.LogInformation("Pedido {PedidoId} confirmado com sucesso", paymentOrder.PedidoId);
                    }
                    else if (status.ToLower() == "cancelled" || status.ToLower() == "rejected")
                    {
                        _logger.LogInformation("Pagamento cancelado/rejeitado. Pedido {PedidoId} não será confirmado", paymentOrder.PedidoId);
                    }

                    await _paymentOrderRepository.AtualizarAsync(paymentOrder);
                    _logger.LogInformation("Status atualizado para PaymentId: {PaymentId}, Novo Status: {Status}", 
                        paymentId, status);
                }
                else
                {
                    _logger.LogWarning("Relação Payment-Order não encontrada para PaymentId: {PaymentId}", paymentId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status for payment {PaymentId}", paymentId);
                throw;
            }
        }
    }
}
