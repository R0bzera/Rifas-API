using Microsoft.EntityFrameworkCore;
using Rifa.Application.Interfaces;
using Rifa.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifa.Infrastructure.Repository
{
    public class WorkerRepository :IWorkerRepository
    {
        private readonly RifaDbContext _context;

        public WorkerRepository(RifaDbContext context)
        {
            _context = context;
        }

        public async Task ProcessarPagamentosPendentes()
        {
            var limite = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(-5), DateTimeKind.Unspecified);


            var rifasParaFinalizar = await _context.cotas_rifa
                .Join(
                    _context.pedidos_rifa,
                    cr => cr.RifaId,
                    pr => pr.RifaId,
                    (cr, pr) => new { cr, pr }
                )
                .Where(x =>
                    x.cr.DataAlteracao <= limite &&
                    x.cr.UsuarioId != null &&
                    x.pr.PagamentoConfirmado == false
                )
                .Select(x => x.pr.Id)
                .ToListAsync();

            foreach (var item in rifasParaFinalizar)
            {
                var paymentOrder = await _context.payment_orders.FirstOrDefaultAsync(p => p.PedidoId == item);
                var cotasRifa = await _context.cotas_rifa.FirstOrDefaultAsync(p => p.PedidoId == item);
                paymentOrder.Status = "expired";
                cotasRifa.PedidoId = null;
                cotasRifa.UsuarioId = null;
                await _context.SaveChangesAsync();
            }

        }
    }
}
