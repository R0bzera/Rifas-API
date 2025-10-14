using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Dto.Pedido;
using Rifa.Application.Interfaces;
using Rifa.Domain.Pagamentos;
using Microsoft.Extensions.Logging;

namespace Rifa.Application.Usecases
{
    public class CreatePixPaymentWithOrderUseCase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPedidoService _pedidoService;
        private readonly IPaymentOrderRepository _paymentOrderRepository;
        private readonly ILogger<CreatePixPaymentWithOrderUseCase> _logger;

        public CreatePixPaymentWithOrderUseCase(
            IPaymentService paymentService,
            IPedidoService pedidoService,
            IPaymentOrderRepository paymentOrderRepository,
            ILogger<CreatePixPaymentWithOrderUseCase> logger)
        {
            _paymentService = paymentService;
            _pedidoService = pedidoService;
            _paymentOrderRepository = paymentOrderRepository;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> ExecuteAsync(PaymentRequestDto dto)
        {
            try
            {
                _logger.LogInformation("Iniciando criação de pagamento PIX com pedido. RifaId: {RifaId}, UsuarioId: {UsuarioId}, QuantidadeCotas: {QuantidadeCotas}", 
                    dto.RifaId, dto.UsuarioId, dto.QuantidadeCotas);

                // 1. Criar pedido primeiro (isso reserva as cotas automaticamente)
                var pedidoDto = new CadastroPedidoDTO
                {
                    RifaId = dto.RifaId,
                    UsuarioId = dto.UsuarioId,
                    TotalCotas = dto.QuantidadeCotas
                };

                _logger.LogInformation("Criando pedido com dados: RifaId: {RifaId}, UsuarioId: {UsuarioId}, TotalCotas: {TotalCotas}", 
                    pedidoDto.RifaId, pedidoDto.UsuarioId, pedidoDto.TotalCotas);

                var pedido = await _pedidoService.CriarAsync(pedidoDto);
                _logger.LogInformation("Pedido criado com sucesso. PedidoId: {PedidoId}, NumerosSorte: {NumerosSorte}", 
                    pedido.Id, string.Join(", ", pedido.Cotas.Select(c => c.Numero)));

                // 2. Criar pagamento PIX
                _logger.LogInformation("Criando pagamento PIX no Mercado Pago...");
                var paymentResponse = await _paymentService.CreatePixPaymentAsync(dto);
                _logger.LogInformation("Pagamento PIX criado com sucesso. PaymentId: {PaymentId}", paymentResponse.Id);

                // 3. Criar relação entre pagamento e pedido
                var paymentOrder = new PaymentOrder
                {
                    Id = Guid.NewGuid(),
                    PaymentId = paymentResponse.Id ?? 0,
                    PedidoId = pedido.Id,
                    Status = paymentResponse.Status ?? "pending",
                    DataCriacao = DateTime.Now,
                    DataAlteracao = DateTime.Now
                };

                await _paymentOrderRepository.CriarAsync(paymentOrder);
                _logger.LogInformation("Relação Payment-Order criada. PaymentOrderId: {PaymentOrderId}", paymentOrder.Id);

                // 4. Combinar informações do pedido com o pagamento
                var response = new PaymentResponseDto
                {
                    // Dados do pagamento PIX
                    Id = paymentResponse.Id,
                    Status = paymentResponse.Status,
                    TicketUrl = paymentResponse.TicketUrl,
                    QrCode = paymentResponse.QrCode,
                    QrCodeBase64 = paymentResponse.QrCodeBase64,
                    DateCreated = paymentResponse.DateCreated,
                    DateApproved = paymentResponse.DateApproved,
                    TransactionAmount = paymentResponse.TransactionAmount,
                    Description = paymentResponse.Description,
                    StatusDetail = paymentResponse.StatusDetail,
                    
                    // Dados do pedido e cotas
                    PedidoId = pedido.Id,
                    RifaTitulo = pedido.RifaTitulo,
                    QuantidadeCotas = pedido.TotalCotas,
                    NumerosSorte = pedido.Cotas.Select(c => c.Numero).ToList(),
                    PagamentoConfirmado = pedido.PagamentoConfirmado
                };

                _logger.LogInformation("Pagamento PIX com pedido criado com sucesso. PedidoId: {PedidoId}, PaymentId: {PaymentId}, NumerosSorte: {NumerosSorte}", 
                    pedido.Id, paymentResponse.Id, string.Join(", ", response.NumerosSorte));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pagamento PIX com pedido. RifaId: {RifaId}, UsuarioId: {UsuarioId}", 
                    dto.RifaId, dto.UsuarioId);
                throw;
            }
        }
    }
}
