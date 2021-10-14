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
        private readonly IMediator _mediator;
        public CreateProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            Product product = new()
            {
                Id = request.ProductId,
                Name = request.Name,
                Value = request.Value,
                Currency = request.Currency,
                Limit = request.Limit
            };

            _database.Products.Add(product);
            Database.SaveChanges();

            ProductCreated productCreated = new(request.ProductId, request.Name, request.Value, request.Currency, request.Limit);
            await _mediator.Publish(productCreated, cancellationToken);

            Database.SaveChanges();
            return Unit.Value;
        }


    }
}
