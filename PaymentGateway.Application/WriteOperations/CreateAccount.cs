using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Linq;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccount : IWriteOperation<CreateAccountCommand>
    {
        // Account account = new Account();

        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;
        public CreateAccount(IEventSender eventSender, AccountOptions accountOptions)
        {
           _eventSender = eventSender;
            _accountOptions = accountOptions;
        }

        public void PerformOperation(CreateAccountCommand operation, Database database)
        {
            var Random = new Random();
            Account account = new Account();
            Person person;

            if (operation.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.Id == operation.PersonId);
            }
            else
            {
                person = database.Persons.FirstOrDefault(x => x.Cnp == operation.Cnp);
            }
            if (person == null)
            {
                throw new Exception("Person not found");
            }

            account.PersonId = person.Id;


            account.Balance = _accountOptions.InitialBalance;
            account.Currency = operation.Currency;
            account.IbanCode = operation.IBanCode;
            account.Status = AccountStatus.Active;
        

            if (operation.AccountType == "Currency")
            {
                account.Type = AccountType.Current;
            }
            else if (operation.AccountType == "Economy")
            {
                account.Type = AccountType.Economy;
            }
            else if (operation.AccountType == "Investment")
            {
                account.Type = AccountType.Investment;
            }
            else
            {
                throw new Exception("Unsupported account type");
            }
            database.Account.Add(account);
            database.SaveChanges();

            AccountCreated accoutCreated = new(account.IbanCode, account.Currency, account.Balance);
            _eventSender.SendEvent(accoutCreated);
        }
    }
}