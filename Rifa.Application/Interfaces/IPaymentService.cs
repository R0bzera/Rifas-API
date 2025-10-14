
using Rifa.Application.Dto.Pagamentos;

namespace Rifa.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePixPaymentAsync(PaymentRequestDto request);
        Task<PaymentResponseDto> GetPaymentStatusAsync(long paymentId);
        Task<bool> ValidateWebhookSignatureAsync(string signature, string payload);
    }
}
