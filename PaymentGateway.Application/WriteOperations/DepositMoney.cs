using Abstractions;
using System;
using PaymentGateway.PublishedLanguage.WriteSide;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;

namespace PaymentGateway.Application.WriteOperations
{
    public class DepositMoney : IWriteOperation<DepositMoneyCommand>
    {
       
        public IEventSender eventSender;
        public DepositMoney(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(DepositMoneyCommand operation, Database database)
        {
            Account account;

            if (operation.AccountId.HasValue)
            {
                account = database.Account.FirstOrDefault(x => x.Id == operation.AccountId);
            }
            else
            {
                account = database.Account.FirstOrDefault(x => x.IbanCode == operation.Iban);
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
                account = database.Account.FirstOrDefault(x => x.Currency == operation.Currency);
            }
           // account.PersonId = account.Id;
            //account.Balance = account.Balance + operation.Value;

            Transaction transaction = new Transaction();
            transaction.Currency = operation.Currency;
            transaction.Amount = operation.Value;
            transaction.Type = TransactionType.Deposit;
            transaction.Date = DateTime.Now;

            account.IbanCode = operation.Iban;

            database.Transactions.Add(transaction);

            account.Balance = account.Balance + transaction.Amount;

            database.SaveChanges();

            DepositCreated depositCreated = new(account.IbanCode, account.Balance, account.Currency);
            eventSender.SendEvent(depositCreated);
        }
    }
}
