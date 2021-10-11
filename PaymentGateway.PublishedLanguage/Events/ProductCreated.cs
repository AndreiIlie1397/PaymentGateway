using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class ProductCreated
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public int Limit { get; set; }
       public ProductCreated(string name, double value, string currency, int limit)
        {
            this.Name = name;
            this.Value = value;
            this.Currency = currency;
            this.Limit = limit;
        }
    }
}
