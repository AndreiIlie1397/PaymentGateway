using Abstractions;
using System;
using PaymentGateway.PublishedLanguage.WriteSide;
using System.Linq;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;

namespace PaymentGateway.Application.WriteOperations
{
    public class DepositMoneyOperation : IWriteOperation<DepositMoneyCommand>
    {

        private readonly IEventSender _eventSender;
        public DepositMoneyOperation(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }
        public void PerformOperation(DepositMoneyCommand operation)
        {
            Database database = Database.GetInstance();
            Account account;

            if (operation.AccountId.HasValue)
            {
                account = database.Accounts.FirstOrDefault(x => x.Id == operation.AccountId);
            }
            else
            {
                account = database.Accounts.FirstOrDefault(x => x.IbanCode == operation.Iban);
            }
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if(operation.Value <= 0)
            {
                throw new Exception("Cannot deposit negative amount");
            }

            if(!String.IsNullOrEmpty(operation.Currency))
            {
                account = database.Accounts.FirstOrDefault(x => x.Currency == operation.Currency);
            }
            account.Id = operation.AccountId;
            account.Balance = account.Balance + operation.Value;
            account.IbanCode = operation.Iban;

            Transaction transaction = new Transaction();
            transaction.Currency = operation.Currency;
            transaction.Amount = operation.Value;
            transaction.Type = TransactionType.Deposit;
            transaction.Date = operation.DateOfTransaction;

            database.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(operation.Value, operation.Currency, operation.DateOfTransaction);
            _eventSender.SendEvent(transactionCreated);

            DepositCreated depositCreated = new(operation.Iban, account.Balance, account.Currency);
            _eventSender.SendEvent(depositCreated);

            database.SaveChanges();
        }
    }
}
