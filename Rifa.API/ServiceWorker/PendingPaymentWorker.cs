using Rifa.Application.Interfaces;

namespace Rifa.API.ServiceWorker
{
    public class PendingPaymentWorker : BackgroundService
    {
        private readonly ILogger<PendingPaymentWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PendingPaymentWorker(
        ILogger<PendingPaymentWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var workerApp = scope
                    .ServiceProvider
                    .GetRequiredService<IWorkerService>();

                _logger.LogInformation("Executando fluxo...");

                    await workerApp.ProcessarPagamentosPendentes();
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
}
