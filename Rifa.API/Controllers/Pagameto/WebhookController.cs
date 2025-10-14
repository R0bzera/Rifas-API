using Microsoft.AspNetCore.Mvc;
using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;

namespace Rifa.API.Controllers.Pagameto
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentStatusService _paymentStatusService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IPaymentService paymentService, 
            IPaymentStatusService paymentStatusService,
            ILogger<WebhookController> logger)
        {
            _paymentService = paymentService;
            _paymentStatusService = paymentStatusService;
            _logger = logger;
        }

        [HttpPost("mercado-pago")]
        public async Task<IActionResult> MercadoPagoWebhook()
        {
            try
            {
                // Lê o payload do body da requisição
                using var reader = new StreamReader(Request.Body);
                var payload = await reader.ReadToEndAsync();

                // Valida a assinatura do webhook
                var signature = Request.Headers["x-signature"].FirstOrDefault();
                if (string.IsNullOrEmpty(signature))
                {
                    _logger.LogWarning("Webhook received without signature");
                    return Unauthorized("Signature missing");
                }

                var isValidSignature = await _paymentService.ValidateWebhookSignatureAsync(signature, payload);
                if (!isValidSignature)
                {
                    _logger.LogWarning("Invalid webhook signature");
                    return Unauthorized("Invalid signature");
                }

                // Deserializa a notificação
                var notification = System.Text.Json.JsonSerializer.Deserialize<WebhookNotificationDto>(payload);
                if (notification == null)
                {
                    _logger.LogWarning("Failed to deserialize webhook notification");
                    return BadRequest("Invalid notification format");
                }

                // Processa a notificação
                await _paymentStatusService.ProcessWebhookNotificationAsync(notification);

                _logger.LogInformation("Webhook processed successfully for payment {PaymentId}", 
                    notification.Data?.Id);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook notification");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "Webhook endpoint is healthy", timestamp = DateTime.UtcNow });
        }
    }
}
