using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentGateway.PublishedLanguage.WriteSide.PurchaseProductCommand;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class ProductPurchased
    {
            public List<PurchaseProductDetail> ProductDetails = new List<PurchaseProductDetail>();
    }
}
