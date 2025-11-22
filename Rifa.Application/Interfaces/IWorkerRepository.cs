using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifa.Application.Interfaces
{
    public interface IWorkerRepository
    {
        Task ProcessarPagamentosPendentes();
    }
}
