using Rifa.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifa.Application.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IWorkerRepository _workerRepository;

        public WorkerService(IWorkerRepository workerRepository) 
        {
            _workerRepository = workerRepository;
        }

        public async Task ProcessarPagamentosPendentes()
        {
            await _workerRepository.ProcessarPagamentosPendentes();
        }
    }
}
