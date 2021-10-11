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
        public string Currency { get; set; }
        //public string Status { get; set; }
        public double Balance { get; set; }

        public AccountCreated(string iban, string currency, double balance)
        {
            this.Iban = iban;
            this.Currency = currency;
           // this.Status = status;
            this.Balance = balance;
        }
    }
}
