namespace Rifa.Application.Interfaces
{
    public interface IWorkerRepository
    {
        Task ProcessarPagamentosPendentes();
    }
}
