using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

// Alias para evitar conflito de nomes com sua entidade de domínio
using MpPayment = MercadoPago.Resource.Payment.Payment;

namespace Rifa.Infrastructure.ExternalsCalls
{
    public class MercadoPagoPaymentService : IPaymentService
    {
        private readonly ILogger<MercadoPagoPaymentService> _logger;
        private readonly string _webhookSecret;

        public MercadoPagoPaymentService(IConfiguration configuration, ILogger<MercadoPagoPaymentService> logger)
        {
            _logger = logger;
            
            var accessToken = configuration["MercadoPago:AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException(nameof(configuration), "AccessToken não configurado no appsettings.json.");
            
            _webhookSecret = configuration["MercadoPago:WebhookSecret"] ?? string.Empty;
            MercadoPagoConfig.AccessToken = accessToken;
        }

        public async Task<PaymentResponseDto> CreatePixPaymentAsync(PaymentRequestDto request)
        {
            try
            {
                if (request is null)
                    throw new ArgumentNullException(nameof(request));

                var idempotencyKey = Guid.NewGuid().ToString();
                var options = new RequestOptions();
                options.CustomHeaders.Add("x-idempotency-key", idempotencyKey);

                var paymentRequest = new PaymentCreateRequest
                {
                    TransactionAmount = request.Amount,
                    Description = request.Description,
                    PaymentMethodId = "pix",
                    Payer = new PaymentPayerRequest
                    {
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Identification = new IdentificationRequest
                        {
                            Type = request.DocumentType,
                            Number = request.DocumentNumber
                        }
                    },
                    DateOfExpiration = DateTime.Now.AddHours(24) // Expira em 24h
                };

                var client = new PaymentClient();
                MpPayment payment = await client.CreateAsync(paymentRequest, options);

                _logger.LogInformation("PIX Payment created successfully. PaymentId: {PaymentId}, Status: {Status}", 
                    payment.Id, payment.Status);

                return new PaymentResponseDto
                {
                    Id = payment.Id,
                    Status = payment.Status ?? string.Empty,
                    TicketUrl = payment.PointOfInteraction?.TransactionData?.TicketUrl ?? string.Empty,
                    QrCode = payment.PointOfInteraction?.TransactionData?.QrCode ?? string.Empty,
                    QrCodeBase64 = payment.PointOfInteraction?.TransactionData?.QrCodeBase64 ?? string.Empty,
                    DateCreated = payment.DateCreated,
                    DateApproved = payment.DateApproved,
                    TransactionAmount = payment.TransactionAmount,
                    Description = payment.Description ?? string.Empty,
                    StatusDetail = payment.StatusDetail ?? string.Empty
                };
            }
            catch (MercadoPago.Error.MercadoPagoApiException ex)
            {
                _logger.LogError(ex, "Erro ao criar pagamento PIX. StatusCode: {StatusCode}", ex.StatusCode);
                throw;
            }
        }

        public async Task<PaymentResponseDto> GetPaymentStatusAsync(long paymentId)
        {
            try
            {
                var client = new PaymentClient();
                var payment = await client.GetAsync(paymentId);

                return new PaymentResponseDto
                {
                    Id = payment.Id,
                    Status = payment.Status ?? string.Empty,
                    TicketUrl = payment.PointOfInteraction?.TransactionData?.TicketUrl ?? string.Empty,
                    QrCode = payment.PointOfInteraction?.TransactionData?.QrCode ?? string.Empty,
                    QrCodeBase64 = payment.PointOfInteraction?.TransactionData?.QrCodeBase64 ?? string.Empty,
                    DateCreated = payment.DateCreated,
                    DateApproved = payment.DateApproved,
                    TransactionAmount = payment.TransactionAmount,
                    Description = payment.Description ?? string.Empty,
                    StatusDetail = payment.StatusDetail ?? string.Empty
                };
            }
            catch (MercadoPago.Error.MercadoPagoApiException ex)
            {
                _logger.LogError(ex, "Erro ao buscar status do pagamento {PaymentId}", paymentId);
                throw;
            }
        }

        public Task<bool> ValidateWebhookSignatureAsync(string signature, string payload)
        {
            if (string.IsNullOrEmpty(_webhookSecret) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(payload))
            {
                _logger.LogWarning("Webhook validation failed: missing required parameters");
                return Task.FromResult(false);
            }

            try
            {
                // Remove o prefixo "sha256=" se presente
                var cleanSignature = signature.Replace("sha256=", "", StringComparison.OrdinalIgnoreCase);
                
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret));
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var computedSignature = Convert.ToHexString(computedHash).ToLower();

                var isValid = string.Equals(computedSignature, cleanSignature.ToLower(), StringComparison.OrdinalIgnoreCase);
                
                if (!isValid)
                {
                    _logger.LogWarning("Webhook signature validation failed. Expected: {Expected}, Received: {Received}", 
                        computedSignature, cleanSignature);
                }

                return Task.FromResult(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar assinatura do webhook");
                return Task.FromResult(false);
            }
        }
    }
}