using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IRequestHandler<EnrollCustomerCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;

        public EnrollCustomerOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
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
                person.Type = PersonType.Company;
            }
            else if (request.ClientType == "Individual")
            {
                person.Type = PersonType.Individual;
            }
            else
            {
                throw new Exception("Unsupported person type");
            }
            person.Id = _database.Persons.Count + 1;

            _database.Persons.Add(person);

            Account account = new()
            {
                Type = AccountType.Current,
                Currency = request.Currency,
                Balance = 0,
                IbanCode = random.Next(100000).ToString()
            };

            _database.Accounts.Add(account);

            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType, request.IbanCode);
            await _mediator.Publish(eventCustomerEnroll, cancellationToken);

            Database.SaveChanges();
            return Unit.Value;
        }
    }
}
