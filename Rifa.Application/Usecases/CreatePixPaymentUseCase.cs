using Rifa.Application.Dto.Pagamentos;
using Rifa.Application.Interfaces;
using Rifa.Domain.Pagamentos;
using Rifa.Domain.ValueObjects;

namespace Rifa.Application.Usecases
{
    public class CreatePixPaymentUseCase
    {
        private readonly IPaymentService _paymentService;

        public CreatePixPaymentUseCase(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<PaymentResponseDto> ExecuteAsync(PaymentRequestDto dto)
        {
            var payer = new Payer(dto.Email, dto.FirstName, dto.LastName, dto.DocumentType, dto.DocumentNumber);
            var payment = new Payment(dto.Amount, dto.Description, payer);

            return await _paymentService.CreatePixPaymentAsync(dto);
        }
    }
}
