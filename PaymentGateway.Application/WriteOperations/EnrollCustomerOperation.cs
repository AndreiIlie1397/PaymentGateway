using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;

namespace PaymentGateway.Application.WriteOperations
{
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        private readonly IEventSender _eventSender;

        public EnrollCustomerOperation(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }
        public void PerformOperation(EnrollCustomerCommand operation)
        {
            Database database = Database.GetInstance();
            var random = new Random();

            Person person = new Person();
            person.Name = operation.Name;
            person.Cnp = operation.UniqueIdentifier;

            if(operation.ClientType == "Company")
            {
                person.Type = PersonType.Company;
            }
            else if (operation.ClientType == "Individual")
            {
                person.Type = PersonType.Individual;
            } else
            {
                throw new Exception("Unsupported person type");
            }
           
            database.Persons.Add(person);

            Account account = new Account();

            account.Type = AccountType.Current;
            account.Currency = operation.Currency;
            account.Balance = 0;
            account.IbanCode = random.Next(100000).ToString();

            database.Accounts.Add(account);
            
            CustomerEnrolled eventCustomerEnroll = new(operation.Name, operation.UniqueIdentifier, operation.ClientType, operation.IbanCode);
            _eventSender.SendEvent(eventCustomerEnroll);

            database.SaveChanges();
        }
    }
}
