using Rifa.Application.Dto.Pagamentos;

namespace Rifa.Application.Interfaces
{
    public interface IPaymentStatusService
    {
        Task ProcessWebhookNotificationAsync(WebhookNotificationDto notification);
        Task UpdatePaymentStatusAsync(long paymentId, string status);
    }
}
