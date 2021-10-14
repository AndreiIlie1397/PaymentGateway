using System;
using PaymentGateway.PublishedLanguage.Commands;
using System.Linq;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class DepositMoneyOperation : IRequestHandler<DepositMoneyCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        public DepositMoneyOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            Account account;

            if (request.AccountId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.Id == request.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.Iban);
            }
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (request.Value <= 0)
            {
                throw new Exception("Cannot deposit negative amount");
            }

            if (!String.IsNullOrEmpty(request.Currency))
            {
                account = _database.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }
            account.Id = request.AccountId;
            account.Balance = account.Balance + request.Value;
            account.IbanCode = request.Iban;

            Transaction transaction = new Transaction
            {
                Currency = request.Currency,
                Amount = request.Value,
                Type = TransactionType.Deposit,
                Date = request.DateOfTransaction
            };

            _database.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(request.Value, request.Currency, request.DateOfTransaction);
            await _mediator.Publish(transactionCreated);

            DepositCreated depositCreated = new(request.Iban, account.Balance, account.Currency);
            await _mediator.Publish(depositCreated, cancellationToken);

            //_database.SaveChanges();
            return Unit.Value;
        }
    }
}
