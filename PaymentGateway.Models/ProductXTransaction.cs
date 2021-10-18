using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class ProductXtransaction
    {
        public int ProductId { get; set; }
        public int TransactionId { get; set; }
        public decimal Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
