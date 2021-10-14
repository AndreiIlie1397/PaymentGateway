using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountCreated: INotification
    {
        public string Iban { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public AccountCreated(string iban, string type, string status)
        {
            this.Iban = iban;
            this.Type = status;
            this.Status = status;
        }
    }
}
