using Rifa.Application.Interfaces;

namespace Rifa.API.Services
{
    public class SorteioBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SorteioBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromSeconds(30); // Verificar a cada 30 segundos

        public SorteioBackgroundService(IServiceProvider serviceProvider, ILogger<SorteioBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SorteioBackgroundService iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var sorteioService = scope.ServiceProvider.GetRequiredService<ISorteioAutomaticoService>();
                        
                        // Verificar rifas prontas para sorteio
                        var rifasProntas = await sorteioService.VerificarRifasProntasParaSorteioAsync();
                        
                        if (rifasProntas.Any())
                        {
                            _logger.LogInformation($"Encontradas {rifasProntas.Count()} rifas prontas para sorteio");
                            
                            foreach (var rifaId in rifasProntas)
                            {
                                try
                                {
                                    var sorteioExecutado = await sorteioService.VerificarEExecutarSorteioAutomaticoAsync(rifaId);
                                    if (sorteioExecutado)
                                    {
                                        _logger.LogInformation($"Sorteio automático executado para rifa {rifaId}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Erro ao executar sorteio automático para rifa {rifaId}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no SorteioBackgroundService");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}
