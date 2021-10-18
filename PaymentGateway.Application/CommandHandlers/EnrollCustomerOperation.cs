using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace PaymentGateway.Application.CommandHandlers
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;

        public EnrollCustomerOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }
        public async Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            var random = new Random();

            Person person = new()
            {
                Name = request.Name,
                Cnp = request.UniqueIdentifier
            };

            if (request.ClientType == "Company")
            {
                person.Type = (int)PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                person.Type = (int)PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }

            _dbContext.Persons.Add(person);

            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType);
            await _mediator.Publish(eventCustomerEnroll, cancellationToken);

            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
