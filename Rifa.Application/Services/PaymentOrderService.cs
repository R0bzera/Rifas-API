using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Rifa.Application.Services
{
    public class PaymentOrderService : IPaymentOrderService
    {
        private readonly IPaymentService _paymentService;
        private readonly IPedidoService _pedidoService;
        private readonly IPaymentOrderRepository _paymentOrderRepository;
        private readonly ILogger<PaymentOrderService> _logger;

        public PaymentOrderService(
            IPaymentService paymentService,
            IPedidoService pedidoService,
            IPaymentOrderRepository paymentOrderRepository,
            ILogger<PaymentOrderService> logger)
        {
            _paymentService = paymentService;
            _pedidoService = pedidoService;
            _paymentOrderRepository = paymentOrderRepository;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> GetPaymentWithOrderInfoAsync(long paymentId)
        {
            try
            {
                _logger.LogInformation("Buscando informações do pagamento com pedido. PaymentId: {PaymentId}", paymentId);

                // Buscar status do pagamento
                var paymentStatus = await _paymentService.GetPaymentStatusAsync(paymentId);
                
                // Buscar informações do pedido associado
                var paymentOrder = await _paymentOrderRepository.ObterPorPaymentIdAsync(paymentId);
                
                if (paymentOrder != null)
                {
                    _logger.LogInformation("Pedido encontrado para o pagamento. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                        paymentId, paymentOrder.PedidoId);
                    
                    // Buscar detalhes do pedido
                    var pedido = await _pedidoService.ObterPorIdAsync(paymentOrder.PedidoId);
                    if (pedido != null)
                    {
                        _logger.LogInformation("Detalhes do pedido obtidos. PaymentId: {PaymentId}, PedidoId: {PedidoId}, PagamentoConfirmado: {PagamentoConfirmado}", 
                            paymentId, paymentOrder.PedidoId, pedido.PagamentoConfirmado);
                    }
                }
                else
                {
                    _logger.LogWarning("Nenhum pedido encontrado para o pagamento. PaymentId: {PaymentId}", paymentId);
                }
                
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
                
                if (IsPaymentApproved(paymentStatus.Status))
                {
                    _logger.LogInformation("Pagamento aprovado. Iniciando confirmação do pedido. PaymentId: {PaymentId}", paymentId);
                    
                    // Buscar relação payment-order
                    var paymentOrder = await _paymentOrderRepository.ObterPorPaymentIdAsync(paymentId);
                    
                    if (paymentOrder != null)
                    {
                        _logger.LogInformation("Relação Payment-Order encontrada. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                            paymentId, paymentOrder.PedidoId);
                        
                        // Verificar se o pedido já foi confirmado
                        var pedido = await _pedidoService.ObterPorIdAsync(paymentOrder.PedidoId);
                        
                        if (pedido != null && !pedido.PagamentoConfirmado)
                        {
                            _logger.LogInformation("Confirmando pagamento do pedido. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                                paymentId, paymentOrder.PedidoId);
                            
                            await _pedidoService.ConfirmarPagamentoAsync(paymentOrder.PedidoId);
                            
                            _logger.LogInformation("Pagamento e pedido confirmados com sucesso. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                                paymentId, paymentOrder.PedidoId);
                        }
                        else if (pedido?.PagamentoConfirmado == true)
                        {
                            _logger.LogInformation("Pedido já estava confirmado. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                                paymentId, paymentOrder.PedidoId);
                        }
                        else
                        {
                            _logger.LogWarning("Pedido não encontrado. PaymentId: {PaymentId}, PedidoId: {PedidoId}", 
                                paymentId, paymentOrder.PedidoId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Relação Payment-Order não encontrada. PaymentId: {PaymentId}", paymentId);
                    }
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

        /// <summary>
        /// Verifica se o status do pagamento indica que foi aprovado.
        /// </summary>
        private static bool IsPaymentApproved(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return status.ToLowerInvariant() == "approved";
        }
    }
}
