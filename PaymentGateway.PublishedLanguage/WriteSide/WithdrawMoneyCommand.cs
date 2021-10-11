using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class WithdrawMoneyCommand
    {
        public int? AccountId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public double Value { get; set; }
        public string Iban { get; set; }
    }
}
