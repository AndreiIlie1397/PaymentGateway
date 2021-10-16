using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class CreateProductOperation : IRequestHandler<CreateProductCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;
        public CreateProductOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
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

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            ProductCreated productCreated = new(request.ProductId, request.Name, request.Value, request.Currency, request.Limit);
            await _mediator.Publish(productCreated, cancellationToken);

            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
