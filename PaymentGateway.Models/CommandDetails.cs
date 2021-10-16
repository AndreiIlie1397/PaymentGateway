using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Models
{
    class CommandDetails
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
