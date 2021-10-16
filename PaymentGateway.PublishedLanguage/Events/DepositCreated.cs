using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class DepositCreated : INotification
    {
        public string Iban { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public DepositCreated(string iban, decimal amount, string currency)
        {
            this.Iban = iban;
            this.Amount = amount;
            this.Currency = currency;
        }
    }
}
