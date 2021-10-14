using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoneyOperation : IRequestHandler<WithdrawMoneyCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        public WithdrawMoneyOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
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

            if (account.Balance < request.Value)
            {
                throw new Exception("Cannot withdraw money");
            }

            Transaction transaction = new Transaction
            {
                Currency = request.Currency,
                Amount = -request.Value,
                Type = TransactionType.Withdraw,
                Date = request.DateOfTransaction
            };

            account.Balance = account.Balance - request.Value;

            _database.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(request.Value, request.Currency, request.DateOfTransaction);
            await _mediator.Publish(transactionCreated);

            WithdrawCreated withdrawCreated = new(account.IbanCode, account.Balance, account.Currency);
            await _mediator.Publish(withdrawCreated, cancellationToken);

            //_database.SaveChanges();
            return Unit.Value;
        }

        
    }
}
