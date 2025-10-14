using Rifa.Application.Dto.Cota;
using Rifa.Application.Interfaces;
using Rifa.Domain.Cota;

namespace Rifa.Application.Services
{
    public class CotaService : ICotaService
    {
        private readonly ICotaRepository _cotaRepository;
        private readonly IRifaRepository _rifaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CotaService(
            ICotaRepository cotaRepository,
            IRifaRepository rifaRepository,
            IUsuarioRepository usuarioRepository)
        {
            _cotaRepository = cotaRepository;
            _rifaRepository = rifaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<CotaDTO>> ObterTodasAsync()
        {
            try
            {
                var cotas = await _cotaRepository.ObterTodosAsync();
                return cotas.Select(c => MapToDTO(c));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter cotas: {ex.Message}");
            }
        }

        public async Task<CotaDTO?> ObterPorIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("ID inválido");

                var cota = await _cotaRepository.ObterPorIdAsync(id);
                
                if (cota == null)
                    throw new Exception($"Cota com ID {id} não encontrada");

                return MapToDTO(cota);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter cota: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaDTO>> ObterPorRifaAsync(Guid rifaId)
        {
            try
            {
                if (rifaId == Guid.Empty)
                    throw new Exception("ID da rifa inválido");

                var cotas = await _cotaRepository.ObterPorRifaAsync(rifaId);
                return cotas.Select(c => MapToDTO(c));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter cotas da rifa: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaDTO>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            try
            {
                if (usuarioId == Guid.Empty)
                    throw new Exception("ID do usuário inválido");

                var cotas = await _cotaRepository.ObterPorUsuarioAsync(usuarioId);
                return cotas.Select(c => MapToDTO(c));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter cotas do usuário: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CotaDisponivelDTO>> ObterDisponiveisPorRifaAsync(Guid rifaId)
        {
            try
            {
                if (rifaId == Guid.Empty)
                    throw new Exception("ID da rifa inválido");

                var rifa = await _rifaRepository.ObterPorIdAsync(rifaId);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {rifaId} não encontrada");

                var cotas = await _cotaRepository.ObterDisponiveisPorRifaAsync(rifaId);
                
                return cotas.Select(c => new CotaDisponivelDTO
                {
                    Id = c.Id,
                    Numero = c.Numero,
                    RifaId = c.RifaId,
                    RifaTitulo = rifa.Titulo,
                    Preco = rifa.Preco
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter cotas disponíveis: {ex.Message}");
            }
        }

        public async Task<CotaDTO> CriarAsync(CadastroCotaDTO dto)
        {
            try
            {
                var rifa = await _rifaRepository.ObterPorIdAsync(dto.RifaId);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {dto.RifaId} não encontrada");

                if (rifa.Finalizada)
                    throw new Exception("Não é possível criar cotas para uma rifa finalizada");

                var cotasExistentes = await _cotaRepository.ObterPorRifaAsync(dto.RifaId);
                if (cotasExistentes.Any(c => c.Numero == dto.Numero))
                    throw new Exception($"Já existe uma cota com o número {dto.Numero} nesta rifa");

                var cota = new CotaEntity
                {
                    Numero = dto.Numero,
                    RifaId = dto.RifaId
                };

                var cotaCriada = await _cotaRepository.CriarAsync(cota);
                
                var cotaCompleta = await _cotaRepository.ObterPorIdAsync(cotaCriada.Id);
                return MapToDTO(cotaCompleta!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar cota: {ex.Message}");
            }
        }

        public async Task ComprarCotaAsync(ComprarCotaDTO dto)
        {
            try
            {
                var cota = await _cotaRepository.ObterPorIdAsync(dto.CotaId);
                if (cota == null)
                    throw new Exception($"Cota com ID {dto.CotaId} não encontrada");

                if (!await _cotaRepository.CotaDisponivelAsync(dto.CotaId))
                    throw new Exception("Cota já foi vendida");

                if (!await _usuarioRepository.ExisteAsync(dto.UsuarioId))
                    throw new Exception($"Usuário com ID {dto.UsuarioId} não encontrado");

                var rifa = await _rifaRepository.ObterPorIdAsync(cota.RifaId);
                if (rifa!.Finalizada)
                    throw new Exception("Não é possível comprar cotas de uma rifa finalizada");

                await _cotaRepository.ComprarCotaAsync(dto.CotaId, dto.UsuarioId, Guid.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao comprar cota: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var cota = await _cotaRepository.ObterPorIdAsync(id);
                if (cota == null)
                    throw new Exception($"Cota com ID {id} não encontrada");

                if (cota.UsuarioId != null)
                    throw new Exception("Não é possível excluir uma cota que já foi vendida");

                await _cotaRepository.ExcluirAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir cota: {ex.Message}");
            }
        }

        private CotaDTO MapToDTO(CotaEntity entity)
        {
            return new CotaDTO
            {
                Id = entity.Id,
                Numero = entity.Numero,
                RifaId = entity.RifaId,
                RifaTitulo = entity.Rifa?.Titulo ?? string.Empty,
                UsuarioId = entity.UsuarioId,
                UsuarioNome = entity.Usuario?.Nome,
                PedidoId = entity.PedidoId,
                DataCriacao = entity.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                DataAlteracao = entity.DataAlteracao.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}