using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateAccount : IWriteOperation<CreateAccountCommand>
    {
        // Account account = new Account();

        public IEventSender eventSender;

        public CreateAccount(IEventSender eventSender)
        {
            this.eventSender = eventSender;
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


            account.Balance = 0;
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
            eventSender.SendEvent(accoutCreated);
        }
    }
}