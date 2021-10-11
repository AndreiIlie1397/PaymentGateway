using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class DepositCreated
    {
        public string Iban { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }

        public DepositCreated(string iban, double amount, string currency)
        {
            this.Iban = iban;
            this.Amount = amount;
            this.Currency = currency;
        }
    }
}
