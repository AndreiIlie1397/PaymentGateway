using MediatR;
using System;
using System.Collections.Generic;
using static PaymentGateway.PublishedLanguage.Commands.PurchaseProductCommand;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class ProductPurchased : INotification
    {
            public List<PurchaseProductDetail> ProductDetails = new List<PurchaseProductDetail>();

        public ProductPurchased()
        {
            throw new NotImplementedException();
        }
    }
}
