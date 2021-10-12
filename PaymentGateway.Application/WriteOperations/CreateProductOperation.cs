using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.WriteSide;


namespace PaymentGateway.Application.WriteOperations
{
    public class CreateProductOperation : IWriteOperation<CreateProductCommand>
    {
        private readonly IEventSender _eventSender;
        public CreateProductOperation(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }
        public void PerformOperation(CreateProductCommand operation)
        {
            Database database = Database.GetInstance();
            Product product = new Product();

            product.Id = operation.ProductId;
            product.Name = operation.Name;
            product.Value = operation.Value;
            product.Currency = operation.Currency;
            product.Limit = operation.Limit;

            database.Products.Add(product);
            database.SaveChanges();

            ProductCreated productCreated = new(operation.ProductId, operation.Name, operation.Value, operation.Currency, operation.Limit);
            _eventSender.SendEvent(productCreated);

            database.SaveChanges();
        }
    }
}
