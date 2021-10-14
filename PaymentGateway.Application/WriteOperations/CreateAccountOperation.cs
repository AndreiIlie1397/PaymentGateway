using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccountOperation : IRequestHandler<CreateAccountCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;

        public CreateAccountOperation(IMediator mediator, AccountOptions accountOptions, Database database)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _database = database;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();

            Account account = new Account();
            Person person;

            if (request.PersonId.HasValue)
            {
                person = _database.Persons.FirstOrDefault(x => x.Id == request.PersonId);
            }
            else
            {
                person = _database.Persons?.FirstOrDefault(x => x.Cnp == request.Cnp);
            }
            if (person == null)
            {
                throw new Exception("Person not found");
            }

            account.PersonId = person.Id;


            account.Balance = _accountOptions.InitialBalance;
            account.Currency = request.Currency;
            account.IbanCode = request.IBanCode;
            account.Status = AccountStatus.Active;


            if (request.Type == "Current")
            {
                account.Type = AccountType.Current;
            }
            else if (request.Type == "Economy")
            {
                account.Type = AccountType.Economy;
            }
            else if (request.Type == "Investment")
            {
                account.Type = AccountType.Investment;
            }
            else
            {
                throw new Exception("Unsupported account type");
            }
            _database.Accounts.Add(account);


            AccountCreated accoutCreated = new(request.IBanCode, request.Type, request.Status.ToString());
            await _mediator.Publish(accoutCreated, cancellationToken);

            //_database.SaveChanges();
            return Unit.Value;
        }
    }

}