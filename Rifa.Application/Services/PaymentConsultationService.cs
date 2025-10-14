using Microsoft.Extensions.Logging;
using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;

namespace Rifa.Application.Services
{
    /// <summary>
    /// Serviço responsável por processar consultas de pagamento e atualizar automaticamente
    /// o status do pedido quando o pagamento for aprovado.
    /// </summary>
    public class PaymentConsultationService : IPaymentConsultationService
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentOrderRepository _paymentOrderRepository;
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PaymentConsultationService> _logger;

        public PaymentConsultationService(
            IPaymentService paymentService,
            IPaymentOrderRepository paymentOrderRepository,
            IPedidoService pedidoService,
            ILogger<PaymentConsultationService> logger)
        {
            _paymentService = paymentService;
            _paymentOrderRepository = paymentOrderRepository;
            _pedidoService = pedidoService;
            _logger = logger;
        }

        /// <summary>
        /// Consulta o status do pagamento e atualiza automaticamente o pedido se o pagamento foi aprovado.
        /// </summary>
        /// <param name="paymentId">ID do pagamento no Mercado Pago</param>
        /// <returns>Informações do pagamento e indicação se houve atualização no pedido</returns>
        public async Task<PaymentConsultationResultDto> ConsultarPagamentoEAtualizarStatusAsync(long paymentId)
        {
            try
            {
                _logger.LogInformation("Iniciando consulta de pagamento e verificação de status. PaymentId: {PaymentId}", paymentId);

                // Buscar status do pagamento no Mercado Pago
                var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentId);
                
                _logger.LogInformation("Status do pagamento obtido. PaymentId: {PaymentId}, Status: {Status}", 
                    paymentId, paymentStatus.Status);

                // Buscar relação payment-order no banco de dados
                var paymentOrder = await _paymentOrderRepository.ObterPorPaymentIdAsync(paymentId);
                
                if (paymentOrder == null)
                {
                    _logger.LogWarning("Relação Payment-Order não encontrada. PaymentId: {PaymentId}", paymentId);
                    return new PaymentConsultationResultDto
                    {
                        PaymentInfo = paymentStatus,
                        OrderUpdated = false,
                        Message = "Pagamento encontrado, mas não há pedido associado"
                    };
                }

                _logger.LogInformation("Relação Payment-Order encontrada. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                    paymentId, paymentOrder.PedidoId);

                var orderUpdated = false;
                var message = string.Empty;

                // Verificar se o pagamento foi aprovado e se o pedido ainda não foi confirmado
                if (IsPaymentApproved(paymentStatus.Status))
                {
                    // Verificar se o pedido já foi confirmado
                    var pedido = await _pedidoService.ObterPorIdAsync(paymentOrder.PedidoId);
                    
                    if (pedido != null && !pedido.PagamentoConfirmado)
                    {
                        _logger.LogInformation("Pagamento aprovado e pedido não confirmado. Confirmando pedido. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                            paymentId, paymentOrder.PedidoId);

                        await _pedidoService.ConfirmarPagamentoAsync(paymentOrder.PedidoId);
                        orderUpdated = true;
                        message = "Pagamento aprovado e pedido confirmado automaticamente";
                        
                        _logger.LogInformation("Pedido confirmado com sucesso. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                            paymentId, paymentOrder.PedidoId);
                    }
                    else if (pedido?.PagamentoConfirmado == true)
                    {
                        _logger.LogInformation("Pagamento aprovado, mas pedido já estava confirmado. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                            paymentId, paymentOrder.PedidoId);
                        message = "Pagamento aprovado, pedido já estava confirmado";
                    }
                    else
                    {
                        _logger.LogWarning("Pagamento aprovado, mas pedido não encontrado. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                            paymentId, paymentOrder.PedidoId);
                        message = "Pagamento aprovado, mas pedido não encontrado";
                    }
                }
                else
                {
                    _logger.LogInformation("Pagamento não aprovado. Status: {Status}. PaymentId: {PaymentId}", 
                        paymentStatus.Status, paymentId);
                    message = $"Pagamento não aprovado. Status atual: {paymentStatus.Status}";
                }

                return new PaymentConsultationResultDto
                {
                    PaymentInfo = paymentStatus,
                    OrderUpdated = orderUpdated,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pagamento e atualizar status. PaymentId: {PaymentId}", paymentId);
                throw new InvalidOperationException($"Erro ao consultar pagamento: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se o status do pagamento indica que foi aprovado.
        /// </summary>
        private static bool IsPaymentApproved(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return status.ToLowerInvariant() == "approved";
        }
    }
}
