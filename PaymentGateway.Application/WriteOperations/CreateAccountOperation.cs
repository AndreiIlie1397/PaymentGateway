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
    public class CreateAccountOperation : IWriteOperation<CreateAccountCommand>
    {
        private readonly IEventSender _eventSender;
        private readonly AccountOptions _accountOptions;

        public CreateAccountOperation(IEventSender eventSender, AccountOptions accountOptions)
        {
            _eventSender = eventSender;
            _accountOptions = accountOptions;
        }

        public void PerformOperation(CreateAccountCommand operation)
        {
            Database database = Database.GetInstance();

            Account account = new Account();
            Person person;

            if (operation.PersonId.HasValue)
            {
                person = database.Persons.FirstOrDefault(x => x.Id == operation.PersonId);
            }
            else
            {
                person = database.Persons?.FirstOrDefault(x => x.Cnp == operation.Cnp);
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


            if (operation.Type == "Current")
            {
                account.Type = AccountType.Current;
            }
            else if (operation.Type == "Economy")
            {
                account.Type = AccountType.Economy;
            }
            else if (operation.Type == "Investment")
            {
                account.Type = AccountType.Investment;
            }
            else
            {
                throw new Exception("Unsupported account type");
            }
            database.Accounts.Add(account);


            AccountCreated accoutCreated = new(operation.IBanCode, operation.Type, operation.Status.ToString());
            _eventSender.SendEvent(accoutCreated);

            database.SaveChanges();
        }
    }
}