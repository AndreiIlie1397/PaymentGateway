using MediatR;
using System;
using System.Collections.Generic;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class PurchaseProductCommand : IRequest
    {
        public int? PersonId { get; set; }
        public int? AccountId { get; set; }
        public string Iban { get; set; }
        public string Currency { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public DateTime DateOfOperation { get; set; }

        public List<PurchaseProductDetail> ProductDetails = new();

        public class PurchaseProductDetail
        {
            public int ProductId { get; set; }
            public int TransactionId { get; set; }
            public decimal Quantity { get; set; }
        }

    }
}
