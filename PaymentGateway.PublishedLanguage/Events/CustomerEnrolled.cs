using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class CustomerEnrolled : INotification
    {
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
        public string ClientType { get; set; }
        public string IbanCode { get; set; }

        public CustomerEnrolled(string name, string cnp, string clientType, string iban)
        {
            this.Name = name;
            this.UniqueIdentifier = cnp;
            this.ClientType = clientType;
            this.IbanCode = iban;
        }
    }
}
