using MediatR;

namespace PaymentGateway.PublishedLanguage.Events
{
    public class ProductCreated : INotification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public int Limit { get; set; }

        public ProductCreated(int id, string name, decimal value, string currency, int limit)
        {
            Id = id;
            Name = name;
            Value = value;
            Currency = currency;
            Limit = limit;
        }
    }
}
