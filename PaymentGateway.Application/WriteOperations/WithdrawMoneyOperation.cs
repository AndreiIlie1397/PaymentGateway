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
        private readonly IEventSender _eventSender;
        public WithdrawMoneyOperation(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
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

            Transaction transaction = new Transaction();
            transaction.Currency = request.Currency;
            transaction.Amount = -request.Value;
            transaction.Type = TransactionType.Withdraw;
            transaction.Date = request.DateOfTransaction;

            account.Balance = account.Balance - request.Value;

            _database.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(request.Value, request.Currency, request.DateOfTransaction);
            _eventSender.SendEvent(transactionCreated);

            WithdrawCreated withdrawCreated = new(account.IbanCode, account.Balance, account.Currency);
            _eventSender.SendEvent(withdrawCreated);

            //_database.SaveChanges();
            return Unit.Task;
        }

        
    }
}
