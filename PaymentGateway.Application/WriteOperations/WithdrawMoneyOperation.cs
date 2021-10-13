using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Linq;

namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoneyOperation : IWriteOperation<WithdrawMoneyCommand>
    {
        private readonly Database _database;
        private readonly IEventSender _eventSender;
        public WithdrawMoneyOperation(IEventSender eventSender, Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(WithdrawMoneyCommand operation)
        {
            //Database database = Database.GetInstance();
            Account account;

            if (operation.AccountId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.Id == operation.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == operation.Iban);
            }
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if(account.Balance < operation.Value)
            {
                throw new Exception("Cannot withdraw money");
            }

            Transaction transaction = new Transaction();
            transaction.Currency = operation.Currency;
            transaction.Amount = -operation.Value;
            transaction.Type = TransactionType.Withdraw;
            transaction.Date = operation.DateOfTransaction;

            account.Balance = account.Balance - operation.Value;

            _database.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(operation.Value, operation.Currency, operation.DateOfTransaction);
            _eventSender.SendEvent(transactionCreated);

            WithdrawCreated withdrawCreated = new(account.IbanCode, account.Balance, account.Currency);
            _eventSender.SendEvent(withdrawCreated);

            _database.SaveChanges();   
        }
    }
}
