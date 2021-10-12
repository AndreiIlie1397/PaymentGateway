using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class ProductCreated
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public int Limit { get; set; }

        public ProductCreated(int id, string name, double value, string currency, int limit)
        {
            Id = id;
            Name = name;
            Value = value;
            Currency = currency;
            Limit = limit;
        }
    }
}
