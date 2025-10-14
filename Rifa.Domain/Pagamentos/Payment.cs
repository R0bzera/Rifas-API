using Rifa.Domain.ValueObjects;

namespace Rifa.Domain.Pagamentos
{

    public class Payment
    {
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public Payer Payer { get; private set; }

        public Payment(decimal amount, string description, Payer payer)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor do pagamento deve ser maior que zero.");

            Amount = amount;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Payer = payer ?? throw new ArgumentNullException(nameof(payer));
        }
    }

}
