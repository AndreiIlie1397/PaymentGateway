using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class WithdrawMoney : IWriteOperation<WithdrawMoneyCommand>
    {
        private readonly IEventSender _eventSender;
        public WithdrawMoney(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }
        public void PerformOperation(WithdrawMoneyCommand operation, Database database)
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

            if(account.Balance < operation.Value)
            {
                throw new Exception("Cannot withdraw money");
            }

            Transaction transaction = new Transaction();
            transaction.Currency = operation.Currency;
            transaction.Amount = operation.Value;
            transaction.Type = TransactionType.Withdraw;
            transaction.Date = DateTime.Now;

            account.Balance = account.Balance - operation.Value;

            database.Transactions.Add(transaction);
            database.SaveChanges();

            WithdrawCreated withdrawCreated = new(account.IbanCode, account.Balance, account.Currency);
            _eventSender.SendEvent(withdrawCreated);
        }


    }
}
