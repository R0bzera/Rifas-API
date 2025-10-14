using Rifa.Domain.Pagamentos;

namespace Rifa.Application.Interfaces
{
    public interface IPaymentOrderRepository
    {
        Task<PaymentOrder?> ObterPorPaymentIdAsync(long paymentId);
        Task<PaymentOrder> CriarAsync(PaymentOrder paymentOrder);
        Task<PaymentOrder> AtualizarAsync(PaymentOrder paymentOrder);
        Task SalvarAlteracoesAsync();
    }
}
