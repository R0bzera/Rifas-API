using Rifa.Application.Dto.Rifa;
using Rifa.Application.Interfaces;
using Rifa.Domain.Rifa;

namespace Rifa.Application.Services
{
    public class RifaService : IRifaService
    {
        private readonly IRifaRepository _rifaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICotaRepository _cotaRepository;

        public RifaService(
            IRifaRepository rifaRepository, 
            IUsuarioRepository usuarioRepository,
            ICotaRepository cotaRepository)
        {
            _rifaRepository = rifaRepository;
            _usuarioRepository = usuarioRepository;
            _cotaRepository = cotaRepository;
        }

        public async Task<IEnumerable<RifaDTO>> ObterTodasAsync()
        {
            try
            {
                var rifas = await _rifaRepository.ObterTodosAsync();
                var rifasDTO = new List<RifaDTO>();
                
                foreach (var rifa in rifas)
                {
                    rifasDTO.Add(await MapToDTO(rifa));
                }
                
                return rifasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter rifas: {ex.Message}");
            }
        }

        public async Task<RifaDTO?> ObterPorIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("ID inválido");

                var rifa = await _rifaRepository.ObterPorIdAsync(id);
                
                if (rifa == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                return await MapToDTO(rifa);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter rifa: {ex.Message}");
            }
        }

        public async Task<IEnumerable<RifaDTO>> ObterAtivasAsync()
        {
            try
            {
                var rifas = await _rifaRepository.ObterAtivasAsync();
                var rifasDTO = new List<RifaDTO>();
                
                foreach (var rifa in rifas)
                {
                    rifasDTO.Add(await MapToDTO(rifa));
                }
                
                return rifasDTO;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter rifas ativas: {ex.Message}");
            }
        }

        public async Task<RifaDTO> CriarAsync(CadastroRifaDTO dto)
        {
            try
            {
                if (dto.Preco <= 0)
                    throw new Exception("Preço deve ser maior que zero");

                if (dto.NumCotas <= 0)
                    throw new Exception("Número de cotas deve ser maior que zero");

                if (dto.NumCotas > 100000)
                    throw new Exception("Número de cotas não pode exceder 100.000");

                var rifa = new RifaEntity
                {
                    Titulo = dto.Titulo,
                    Descricao = dto.Descricao,
                    Imagem = dto.Imagem,
                    Preco = dto.Preco,
                    NumCotas = dto.NumCotas,
                    Finalizada = false
                };

                var rifaCriada = await _rifaRepository.CriarAsync(rifa);

                await CriarCotasParaRifa(rifaCriada.Id, dto.NumCotas);

                return await MapToDTO(rifaCriada);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar rifa: {ex.Message}");
            }
        }

        public async Task<RifaDTO> AtualizarAsync(Guid id, AtualizacaoRifaDTO dto)
        {
            try
            {
                var rifaExistente = await _rifaRepository.ObterPorIdAsync(id);
                if (rifaExistente == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                if (rifaExistente.Finalizada)
                    throw new Exception("Não é possível alterar uma rifa finalizada");

                if (dto.Preco <= 0)
                    throw new Exception("Preço deve ser maior que zero");

                if (dto.NumCotas < rifaExistente.NumCotas)
                {
                    var cotasVendidas = (await _cotaRepository.ObterPorRifaAsync(id))
                        .Count(c => c.UsuarioId != null);
                    
                    if (cotasVendidas > dto.NumCotas)
                        throw new Exception($"Não é possível reduzir para {dto.NumCotas} cotas pois já existem {cotasVendidas} cotas vendidas");
                }

                rifaExistente.Titulo = dto.Titulo;
                rifaExistente.Descricao = dto.Descricao;
                rifaExistente.Imagem = dto.Imagem;
                rifaExistente.Preco = dto.Preco;
                rifaExistente.NumCotas = dto.NumCotas;

                var rifaAtualizada = await _rifaRepository.AtualizarAsync(rifaExistente);
                return await MapToDTO(rifaAtualizada);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar rifa: {ex.Message}");
            }
        }

        public async Task FinalizarAsync(Guid id, FinalizarRifaDTO dto)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(id);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                if (rifa.Finalizada)
                    throw new Exception("Rifa já está finalizada");

                if (!await _usuarioRepository.ExisteAsync(dto.GanhadorId))
                    throw new Exception($"Usuário ganhador com ID {dto.GanhadorId} não encontrado");

                var cotasDoGanhador = await _cotaRepository.ObterPorRifaAsync(id);
                if (!cotasDoGanhador.Any(c => c.UsuarioId == dto.GanhadorId))
                    throw new Exception("O ganhador deve ter comprado pelo menos uma cota desta rifa");

                await _rifaRepository.FinalizarAsync(id, dto.GanhadorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao finalizar rifa: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(id);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                var cotasVendidas = (await _cotaRepository.ObterPorRifaAsync(id))
                    .Any(c => c.UsuarioId != null);

                if (cotasVendidas)
                    throw new Exception("Não é possível excluir uma rifa com cotas vendidas");

                await _rifaRepository.ExcluirAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir rifa: {ex.Message}");
            }
        }

        private async Task CriarCotasParaRifa(Guid rifaId, int numCotas)
        {
            for (int i = 1; i <= numCotas; i++)
            {
                var cota = new Domain.Cota.CotaEntity
                {
                    Numero = i,
                    RifaId = rifaId,
                    DataCriacao = DateTime.Now,
                    DataAlteracao = DateTime.Now
                };
                await _cotaRepository.CriarAsync(cota);
            }
        }

        public async Task<SorteioResultadoDTO> SortearRifaAsync(Guid id)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(id);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                if (rifa.Finalizada)
                    throw new Exception("Rifa já foi sorteada");

                // Verificar se todas as cotas foram vendidas
                var cotas = await _cotaRepository.ObterPorRifaAsync(id);
                var cotasVendidas = cotas.Count(c => c.UsuarioId != null);
                
                if (cotasVendidas != rifa.NumCotas)
                    throw new Exception($"Rifa não está completa. Vendidas: {cotasVendidas}/{rifa.NumCotas}");

                // Sortear número aleatório
                var random = new Random();
                var numeroSorteado = random.Next(1, rifa.NumCotas + 1);

                // Encontrar o ganhador
                var cotaGanhadora = cotas.FirstOrDefault(c => c.Numero == numeroSorteado);
                if (cotaGanhadora?.UsuarioId == null)
                    throw new Exception("Erro ao encontrar ganhador");

                var ganhador = await _usuarioRepository.ObterPorIdAsync(cotaGanhadora.UsuarioId.Value);
                if (ganhador == null)
                    throw new Exception("Ganhador não encontrado");

                // Finalizar rifa
                await _rifaRepository.FinalizarAsync(id, cotaGanhadora.UsuarioId.Value);

                return new SorteioResultadoDTO
                {
                    RifaId = id,
                    RifaTitulo = rifa.Titulo,
                    NumeroSorteado = numeroSorteado,
                    GanhadorId = ganhador.Id,
                    GanhadorNome = ganhador.Nome,
                    GanhadorEmail = ganhador.Email,
                    DataSorteio = DateTime.Now,
                    SorteioRealizado = true,
                    Mensagem = $"Número {numeroSorteado} foi sorteado! Ganhador: {ganhador.Nome}"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao sortear rifa: {ex.Message}");
            }
        }

        public async Task<NumeroSorteadoDTO> GerarNumeroSorteadoAsync(Guid id)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(id);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                if (rifa.Finalizada)
                    throw new Exception("Rifa já foi sorteada");

                // Verificar se todas as cotas foram vendidas
                var cotas = await _cotaRepository.ObterPorRifaAsync(id);
                var cotasVendidas = cotas.Count(c => c.UsuarioId != null);
                
                if (cotasVendidas != rifa.NumCotas)
                    throw new Exception($"Rifa não está completa. Vendidas: {cotasVendidas}/{rifa.NumCotas}");

                // Gerar número aleatório entre 1 e NumCotas
                var random = new Random();
                var numeroSorteado = random.Next(1, rifa.NumCotas + 1);

                return new NumeroSorteadoDTO
                {
                    RifaId = id,
                    NumeroSorteado = numeroSorteado,
                    TotalCotas = rifa.NumCotas,
                    DataGeracao = DateTime.Now,
                    Mensagem = $"Número {numeroSorteado} gerado para rifa com {rifa.NumCotas} cotas"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao gerar número sorteado: {ex.Message}");
            }
        }

        public async Task<StatusSorteioDTO> ObterStatusSorteioAsync(Guid id)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(id);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {id} não encontrada");

                var cotas = await _cotaRepository.ObterPorRifaAsync(id);
                var cotasVendidas = cotas.Count(c => c.UsuarioId != null);
                var cotasDisponiveis = rifa.NumCotas - cotasVendidas;
                var rifaCompleta = cotasVendidas == rifa.NumCotas;

                string status;
                int tempoRestanteSegundos = 0;

                if (rifa.Finalizada)
                {
                    status = "Sorteada";
                }
                else if (rifaCompleta)
                {
                    status = "Pronta para sorteio";
                }
                else
                {
                    status = "Em andamento";
                }

                return new StatusSorteioDTO
                {
                    RifaId = id,
                    RifaTitulo = rifa.Titulo,
                    NumCotas = rifa.NumCotas,
                    CotasVendidas = cotasVendidas,
                    CotasDisponiveis = cotasDisponiveis,
                    RifaCompleta = rifaCompleta,
                    SorteioIniciado = false,
                    SorteioFinalizado = rifa.Finalizada,
                    DataSorteio = rifa.Finalizada ? rifa.DataAlteracao : null,
                    NumeroSorteado = rifa.Finalizada ? cotas.FirstOrDefault(c => c.UsuarioId == rifa.GanhadorId)?.Numero : null,
                    GanhadorId = rifa.GanhadorId,
                    GanhadorNome = rifa.Ganhador?.Nome,
                    TempoRestanteSegundos = tempoRestanteSegundos,
                    Status = status
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter status do sorteio: {ex.Message}");
            }
        }

        private async Task<RifaDTO> MapToDTO(RifaEntity entity)
        {
            // Calcular cotas disponíveis de forma mais eficiente
            var cotasDisponiveis = await _cotaRepository.ContarCotasDisponiveisPorRifaAsync(entity.Id);

            return new RifaDTO
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Descricao = entity.Descricao,
                Imagem = entity.Imagem,
                Preco = entity.Preco,
                NumCotas = entity.NumCotas,
                CotasDisponiveis = cotasDisponiveis,
                GanhadorId = entity.GanhadorId,
                GanhadorNome = entity.Ganhador?.Nome,
                Finalizada = entity.Finalizada,
                DataCriacao = entity.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                DataAlteracao = entity.DataAlteracao.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}