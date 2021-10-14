using MediatR;
using PaymentGateway.Models;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class CreateAccountCommand: IRequest
    {
        public int? PersonId { get; set; }
        public string IBanCode {get; set;}
        public double Balance { get; set; }
        public string Currency { get; set; }
        public AccountStatus Status { get; set; }
        public string Type { get; set; }
        public string Cnp { get; set; }
        
    }
}
