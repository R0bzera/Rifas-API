using Microsoft.EntityFrameworkCore;
using Rifa.Application.Interfaces;
using Rifa.Domain.Pagamentos;
using Rifa.Infrastructure.Config;

namespace Rifa.Infrastructure.Repository
{
    public class PaymentOrderRepository : IPaymentOrderRepository
    {
        private readonly RifaDbContext _context;

        public PaymentOrderRepository(RifaDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentOrder?> ObterPorPaymentIdAsync(long paymentId)
        {
            return await _context.payment_orders
                .FirstOrDefaultAsync(po => po.PaymentId == paymentId);
        }

        public async Task<PaymentOrder> CriarAsync(PaymentOrder paymentOrder)
        {
            _context.payment_orders.Add(paymentOrder);
            await _context.SaveChangesAsync();
            return paymentOrder;
        }

        public async Task<PaymentOrder> AtualizarAsync(PaymentOrder paymentOrder)
        {
            _context.payment_orders.Update(paymentOrder);
            await _context.SaveChangesAsync();
            return paymentOrder;
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
