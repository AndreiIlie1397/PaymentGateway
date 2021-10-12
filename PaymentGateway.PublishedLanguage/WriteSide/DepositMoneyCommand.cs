using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class DepositMoneyCommand
    {
        public int? AccountId { get; set; }
        public string UniqueIdentifier { get; set; }
        public string Currency { get; set; }
        public double Value { get; set; }
        public string Iban { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public DateTime DateOfOperation { get; set; }
    }
}
