using MediatR;
using System.Collections.Generic;

namespace PaymentGateway.PublishedLanguage.Commands
{
    public class PurchaseProductCommand : IRequest
    {
        public int? PersonId { get; set; }
        public int? AccountId { get; set; }
        public string Iban { get; set; }

        public List<PurchaseProductDetail> ProductDetails = new();

        public class PurchaseProductDetail
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
