using Rifa.Application.Interfaces;

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
