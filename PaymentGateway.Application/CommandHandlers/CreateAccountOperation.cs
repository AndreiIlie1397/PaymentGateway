using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.CommandHandlers
{
    public class CreateAccountOperation : IRequestHandler<CreateAccountCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly AccountOptions _accountOptions;

        public CreateAccountOperation(IMediator mediator, AccountOptions accountOptions, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _accountOptions = accountOptions;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            Account account = new();
            Person person;

            if (request.PersonId.HasValue)
            {
                person = _dbContext.Persons.FirstOrDefault(x => x.Id == request.PersonId);
            }
            else
            {
                person = _dbContext.Persons?.FirstOrDefault(x => x.Cnp == request.Cnp);
            }
            if (person == null)
            {
                throw new Exception("Person not found");
            }

            account.PersonId = person.Id;

            account.Balance = _accountOptions.InitialBalance;
            account.Currency = request.Currency;
            account.Iban = request.Iban;
            account.Status = (int)AccountStatus.Active;
            account.Limit = 100000;

            if (request.Type == "Current")
            {
                account.Type = (int)AccountType.Current;
            }
            else if (request.Type == "Economy")
            {
                account.Type = (int)AccountType.Economy;
            }
            else if (request.Type == "Investment")
            {
                account.Type = (int)AccountType.Investment;
            }
            else
            {
                throw new Exception("Unsupported account type");
            }
            _dbContext.Accounts.Add(account);

            AccountCreated accoutCreated = new(request.Iban, request.Type, request.Status.ToString());
            await _mediator.Publish(accoutCreated, cancellationToken);

            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }

}