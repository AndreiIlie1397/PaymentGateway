using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class PurchaseProductCommand
    {
        public int? PersonId { get; set; }
        public string Iban { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }

        public DateTime Data { get; set; }
        public List<PurchaseProductDetail> ProductDetails = new List<PurchaseProductDetail>();

        public class PurchaseProductDetail
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }

        }

    }
}
