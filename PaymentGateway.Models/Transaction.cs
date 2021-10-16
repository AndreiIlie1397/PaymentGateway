using System;

namespace PaymentGateway.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public TransactionType Type { get; set; }
        public int? AccountId { get; set; }
    }
}
