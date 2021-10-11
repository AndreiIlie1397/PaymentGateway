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
    public class EnrollCustomerOperation : IWriteOperation<EnrollCustomerCommand>
    {
        public IEventSender eventSender;


        public EnrollCustomerOperation(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(EnrollCustomerCommand operation, Database database)
        {
            var Random = new Random();
           
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
            account.IbanCode = operation.IbanCode;

            database.Account.Add(account);
            database.SaveChanges();

            CustomerEnrolled eventCustEnroll = new(operation.Name, operation.UniqueIdentifier, operation.ClientType, operation.IbanCode);
            eventSender.SendEvent(eventCustEnroll);
        }
    }
}
