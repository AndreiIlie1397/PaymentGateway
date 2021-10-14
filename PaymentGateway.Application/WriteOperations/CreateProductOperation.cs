using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateProductOperation : IRequestHandler<CreateProductCommand>
    {
        private readonly Database _database;
        private readonly IEventSender _eventSender;
        public CreateProductOperation(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            Product product = new Product();

            product.Id = request.ProductId;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
            product.Limit = request.Limit;

            _database.Products.Add(product);
            _database.SaveChanges();

            ProductCreated productCreated = new(request.ProductId, request.Name, request.Value, request.Currency, request.Limit);
            _eventSender.SendEvent(productCreated);

            //_database.SaveChanges();
            return Unit.Task;
        }

      
    }
}
