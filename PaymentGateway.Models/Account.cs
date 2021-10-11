using System;

namespace PaymentGateway.Models
{
    public class Account
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public string IbanCode { get; set; }
        public AccountType Type { get; set; }
        public AccountStatus Status { get; set; }
        public double Limit { get; set; }

        public int PersonId { get; set; }
    }
}
