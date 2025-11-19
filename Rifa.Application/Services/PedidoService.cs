using Rifa.Application.Dto.Pedido;
using Rifa.Application.Interfaces;
using Rifa.Domain.Pedido;

namespace Rifa.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IRifaRepository _rifaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICotaRepository _cotaRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IRifaRepository rifaRepository,
            IUsuarioRepository usuarioRepository,
            ICotaRepository cotaRepository)
        {
            _pedidoRepository = pedidoRepository;
            _rifaRepository = rifaRepository;
            _usuarioRepository = usuarioRepository;
            _cotaRepository = cotaRepository;
        }

        public async Task<IEnumerable<PedidoDTO>> ObterTodosAsync()
        {
            try
            {
                var pedidos = await _pedidoRepository.ObterTodosAsync();
                return pedidos.Select(p => MapToDTO(p));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter pedidos: {ex.Message}");
            }
        }

        public async Task<PedidoDTO?> ObterPorIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("ID inválido");

                var pedido = await _pedidoRepository.ObterPorIdAsync(id);
                
                if (pedido == null)
                    throw new Exception($"Pedido com ID {id} não encontrado");

                return MapToDTO(pedido);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter pedido: {ex.Message}");
            }
        }

        public async Task<IEnumerable<PedidoDTO>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            try
            {
                if (usuarioId == Guid.Empty)
                    throw new Exception("ID do usuário inválido");

                var pedidos = await _pedidoRepository.ObterPorUsuarioAsync(usuarioId);
                return pedidos.Select(p => MapToDTO(p));
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter pedidos do usuário: {ex.Message}");
            }
        }

        public async Task<PedidoDTO> CriarAsync(CadastroPedidoDTO dto)
        {
            try
            {
                if (dto.TotalCotas <= 0)
                    throw new Exception("Total de cotas deve ser maior que zero");

                var rifa = await _rifaRepository.ObterPorIdAsync(dto.RifaId);
                if (rifa == null)
                    throw new Exception($"Rifa com ID {dto.RifaId} não encontrada");

                if (rifa.Finalizada)
                    throw new Exception("Não é possível comprar cotas de uma rifa finalizada");

                if (!await _usuarioRepository.ExisteAsync(dto.UsuarioId))
                    throw new Exception($"Usuário com ID {dto.UsuarioId} não encontrado");

                var cotasDisponiveis = await _cotaRepository.ObterDisponiveisPorRifaAsync(dto.RifaId);
                if (cotasDisponiveis.Count() < dto.TotalCotas)
                    throw new Exception($"Apenas {cotasDisponiveis.Count()} cotas disponíveis. Solicitado: {dto.TotalCotas}");

                var pedido = new PedidoEntity
                {
                    RifaId = dto.RifaId,
                    TotalCotas = dto.TotalCotas,
                    UsuarioId = dto.UsuarioId,
                    PagamentoConfirmado = false
                };

                var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);

                await ReservarCotasParaPedido(pedidoCriado.Id, dto.RifaId, dto.UsuarioId, dto.TotalCotas);

                var pedidoCompleto = await _pedidoRepository.ObterPorIdAsync(pedidoCriado.Id);
                return MapToDTO(pedidoCompleto!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar pedido: {ex.Message}");
            }
        }

        public async Task ConfirmarPagamentoAsync(Guid id)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPorIdAsync(id);
                if (pedido == null)
                    throw new Exception($"Pedido com ID {id} não encontrado");

                if (pedido.PagamentoConfirmado)
                    throw new Exception("Pagamento já foi confirmado");

                await _pedidoRepository.ConfirmarPagamentoAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao confirmar pagamento: {ex.Message}");
            }
        }

        public async Task ExcluirAsync(Guid id)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPorIdAsync(id);
                if (pedido == null)
                    throw new Exception($"Pedido com ID {id} não encontrado");

                if (pedido.PagamentoConfirmado)
                    throw new Exception("Não é possível excluir pedido com pagamento confirmado");

                foreach (var cota in pedido.Cotas)
                {
                    cota.UsuarioId = null;
                    cota.PedidoId = null;
                    await _cotaRepository.AtualizarAsync(cota);
                }

                await _pedidoRepository.ExcluirAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir pedido: {ex.Message}");
            }
        }

        private async Task ReservarCotasParaPedido(Guid pedidoId, Guid rifaId, Guid usuarioId, int quantidade)
        {
            var cotasDisponiveis = await _cotaRepository.ObterCotasAleatoriasDisponiveisAsync(rifaId, quantidade);
            
            if (!cotasDisponiveis.Any())
            {
                throw new Exception($"Nenhuma cota disponível encontrada para a rifa {rifaId}");
            }

            foreach (var cota in cotasDisponiveis)
            {
                await _cotaRepository.ComprarCotaAsync(cota.Id, usuarioId, pedidoId);
            }
        }

        private PedidoDTO MapToDTO(PedidoEntity entity)
        {
            var precoUnitario = entity.Rifa?.Preco ?? 0;
            var valorTotal = entity.TotalCotas * precoUnitario;
            
            return new PedidoDTO
            {
                Id = entity.Id,
                RifaId = entity.RifaId,
                RifaTitulo = entity.Rifa?.Titulo ?? string.Empty,
                RifaImagem = entity.Rifa?.Imagem,
                TotalCotas = entity.TotalCotas,
                PrecoUnitario = precoUnitario,
                ValorTotal = valorTotal,
                PagamentoConfirmado = entity.PagamentoConfirmado,
                UsuarioId = entity.UsuarioId,
                UsuarioNome = entity.Usuario?.Nome ?? string.Empty,
                DataCriacao = entity.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                DataAlteracao = entity.DataAlteracao.ToString("yyyy-MM-dd HH:mm:ss"),
                Cotas = entity.Cotas?.Select(c => new Dto.Cota.CotaDTO
                {
                    Id = c.Id,
                    Numero = c.Numero,
                    RifaId = c.RifaId,
                    RifaTitulo = c.Rifa?.Titulo ?? string.Empty,
                    UsuarioId = c.UsuarioId,
                    UsuarioNome = c.Usuario?.Nome,
                    PedidoId = c.PedidoId,
                    DataCriacao = c.DataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                    DataAlteracao = c.DataAlteracao.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList() ?? new List<Dto.Cota.CotaDTO>()
            };
        }
    }
}