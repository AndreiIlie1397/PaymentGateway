using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class AccountCreated
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
