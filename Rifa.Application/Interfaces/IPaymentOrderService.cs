using Rifa.Application.Dto.Pagamentos;

namespace Rifa.Application.Interfaces
{
    public interface IPaymentOrderService
    {
        Task<PaymentResponseDto> GetPaymentWithOrderInfoAsync(long paymentId);
        Task ConfirmPaymentAndOrderAsync(long paymentId);
    }
}
