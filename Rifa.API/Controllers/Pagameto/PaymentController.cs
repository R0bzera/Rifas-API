using Microsoft.AspNetCore.Mvc;
using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Usecases;
using Rifa.Application.Interfaces;

namespace Rifa.API.Controllers.Pagameto
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly CreatePixPaymentUseCase _createPixPaymentUseCase;
        private readonly CreatePixPaymentWithOrderUseCase _createPixPaymentWithOrderUseCase;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentConsultationService _paymentConsultationService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            CreatePixPaymentUseCase createPixPaymentUseCase,
            CreatePixPaymentWithOrderUseCase createPixPaymentWithOrderUseCase,
            IPaymentService paymentService,
            IPaymentConsultationService paymentConsultationService,
            ILogger<PaymentController> logger)
        {
            _createPixPaymentUseCase = createPixPaymentUseCase;
            _createPixPaymentWithOrderUseCase = createPixPaymentWithOrderUseCase;
            _paymentService = paymentService;
            _paymentConsultationService = paymentConsultationService;
            _logger = logger;
        }

        [HttpPost("pix")]
        public async Task<IActionResult> CreatePixPayment([FromBody] PaymentRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid payment request data");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Payment request received. RifaId: {RifaId}, UsuarioId: {UsuarioId}, QuantidadeCotas: {QuantidadeCotas}", 
                    dto.RifaId, dto.UsuarioId, dto.QuantidadeCotas);
                
                _logger.LogInformation("Raw values - RifaId: {RifaId}, UsuarioId: {UsuarioId}, QuantidadeCotas: {QuantidadeCotas}", 
                    dto.RifaId.ToString(), dto.UsuarioId.ToString(), dto.QuantidadeCotas);

                // Validar dados obrigatórios para o novo fluxo
                if (dto.RifaId == Guid.Empty)
                {
                    _logger.LogWarning("RifaId is empty, using old flow");
                    var simpleResult = await _createPixPaymentUseCase.ExecuteAsync(dto);
                    return Ok(simpleResult);
                }

                if (dto.UsuarioId == Guid.Empty)
                {
                    _logger.LogWarning("UsuarioId is empty (Guid.Empty), using old flow");
                    _logger.LogWarning("This usually means the user is not logged in or userId is not being sent correctly");
                    var simpleResult = await _createPixPaymentUseCase.ExecuteAsync(dto);
                    return Ok(simpleResult);
                }

                if (dto.QuantidadeCotas <= 0)
                {
                    _logger.LogWarning("QuantidadeCotas is invalid, using old flow");
                    var simpleResult = await _createPixPaymentUseCase.ExecuteAsync(dto);
                    return Ok(simpleResult);
                }

                // TEMPORÁRIO: Sempre usar novo fluxo para teste
                _logger.LogInformation("Using new flow: PIX payment with order (temporary for testing)");
                
                // Novo fluxo: pagamento com rifa (cria pedido + reserva cotas + pagamento PIX)
                var orderResult = await _createPixPaymentWithOrderUseCase.ExecuteAsync(dto);
                
                _logger.LogInformation("PIX payment with order created successfully. PaymentId: {PaymentId}, PedidoId: {PedidoId}, NumerosSorte: {NumerosSorte}", 
                    orderResult.Id, orderResult.PedidoId, string.Join(", ", orderResult.NumerosSorte));
                
                return Ok(orderResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PIX payment");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("consultar/{paymentId:long}")]
        public async Task<IActionResult> ConsultarPagamento(long paymentId)
        {
            try
            {
                if (paymentId <= 0)
                {
                    _logger.LogWarning("Invalid payment ID provided: {PaymentId}", paymentId);
                    return BadRequest("ID do pagamento inválido");
                }

                _logger.LogInformation("Iniciando consulta de pagamento com atualização automática. PaymentId: {PaymentId}", paymentId);

                // Usar o novo serviço que consulta e atualiza automaticamente o status
                var result = await _paymentConsultationService.ConsultarPagamentoEAtualizarStatusAsync(paymentId);
                
                _logger.LogInformation("Consulta de pagamento concluída. PaymentId: {PaymentId}, Status: {Status}, OrderUpdated: {OrderUpdated}", 
                    paymentId, result.PaymentInfo.Status, result.OrderUpdated);
                
                return Ok(new
                {
                    payment = result.PaymentInfo,
                    orderUpdated = result.OrderUpdated,
                    message = result.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving and updating payment status for PaymentId: {PaymentId}", paymentId);
                return StatusCode(500, new
                {
                    message = "Erro interno do servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Consulta apenas o status do pagamento sem atualizar o pedido automaticamente.
        /// </summary>
        [HttpGet("status/{paymentId:long}")]
        public async Task<IActionResult> ConsultarStatusPagamento(long paymentId)
        {
            try
            {
                if (paymentId <= 0)
                {
                    _logger.LogWarning("Invalid payment ID provided: {PaymentId}", paymentId);
                    return BadRequest("ID do pagamento inválido");
                }

                _logger.LogInformation("Consultando apenas status do pagamento. PaymentId: {PaymentId}", paymentId);

                var result = await _paymentService.GetPaymentStatusAsync(paymentId);
                
                _logger.LogInformation("Status do pagamento obtido. PaymentId: {PaymentId}, Status: {Status}", 
                    paymentId, result.Status);
                
                return Ok(new
                {
                    payment = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment status for PaymentId: {PaymentId}", paymentId);
                return StatusCode(500, new
                {
                    message = "Erro interno do servidor",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "Payment endpoint is healthy", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }
    }
}
