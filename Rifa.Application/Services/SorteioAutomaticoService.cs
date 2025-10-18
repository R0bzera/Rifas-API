using Rifa.Application.Interfaces;
using Rifa.Application.Dto.Rifa;
using Rifa.Domain.Rifa;

namespace Rifa.Application.Services
{
    public class SorteioAutomaticoService : ISorteioAutomaticoService
    {
        private readonly IRifaRepository _rifaRepository;
        private readonly ICotaRepository _cotaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRifaService _rifaService;

        public SorteioAutomaticoService(
            IRifaRepository rifaRepository,
            ICotaRepository cotaRepository,
            IUsuarioRepository usuarioRepository,
            IRifaService rifaService)
        {
            _rifaRepository = rifaRepository;
            _cotaRepository = cotaRepository;
            _usuarioRepository = usuarioRepository;
            _rifaService = rifaService;
        }

        public async Task<bool> VerificarEExecutarSorteioAutomaticoAsync(Guid rifaId)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(rifaId);
                if (rifa == null || rifa.Finalizada)
                    return false;

                // Verificar se todas as cotas foram vendidas
                var cotas = await _cotaRepository.ObterPorRifaAsync(rifaId);
                var cotasVendidas = cotas.Count(c => c.UsuarioId != null);
                
                if (cotasVendidas != rifa.NumCotas)
                    return false;

                // Executar sorteio automático
                await _rifaService.SortearRifaAsync(rifaId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao executar sorteio automático: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Guid>> VerificarRifasProntasParaSorteioAsync()
        {
            try
            {
                var rifasAtivas = await _rifaRepository.ObterAtivasAsync();
                var rifasProntas = new List<Guid>();

                foreach (var rifa in rifasAtivas)
                {
                    var cotas = await _cotaRepository.ObterPorRifaAsync(rifa.Id);
                    var cotasVendidas = cotas.Count(c => c.UsuarioId != null);
                    
                    if (cotasVendidas == rifa.NumCotas)
                    {
                        rifasProntas.Add(rifa.Id);
                    }
                }

                return rifasProntas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar rifas prontas: {ex.Message}");
            }
        }
    }
}
