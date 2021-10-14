using PaymentGateway.Abstractions;
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
        private readonly IEventSender _eventSender;

        public EnrollCustomerOperation(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(EnrollCustomerCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            var random = new Random();

            Person person = new Person
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

            Account account = new Account();

            account.Type = AccountType.Current;
            account.Currency = request.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            _database.Accounts.Add(account);

            CustomerEnrolled eventCustomerEnroll = new(request.Name, request.UniqueIdentifier, request.ClientType, request.IbanCode);
            _eventSender.SendEvent(eventCustomerEnroll);

            //_database.SaveChanges();
            return Unit.Task;
        }
    }
}
