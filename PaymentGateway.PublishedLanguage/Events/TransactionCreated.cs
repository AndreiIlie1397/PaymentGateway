using MediatR;
using System;

namespace PaymentGateway.PublishedLanguage.Events
{
   public class TransactionCreated : INotification
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }

        public TransactionCreated(decimal amount, string currency, DateTime date)
        {
            Amount = amount;
            Currency = currency;
            Date = date;
        }
    }
}
