using PaymentGateway.Abstractions;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.ExternalService;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using static PaymentGateway.PublishedLanguage.WriteSide.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();
            Account firstAccount = new Account();
            firstAccount.Balance = 100;

            EnrollCustomerCommand command = new EnrollCustomerCommand();
            command.Name = "Ilie Andrei";
            command.UniqueIdentifier = "1234567890";
            command.ClientType = "Individual";
            command.AccountType = "Current";
            command.Currency = "RON";

            IEventSender eventSender = new EventSender();
            EnrollCustomerOperation client = new EnrollCustomerOperation(eventSender);
            client.PerformOperation(command, db);

            





            CreateAccountCommand account = new CreateAccountCommand();
            //account.IBanCode = "RO12INGB132435678";
            account.PersonId = 0;
            account.Currency = "RON";
            account.Cnp = "197231456445";
            account.AccountType = "Currency";
            account.IBanCode = "RO12INGB1234567890";

            Console.WriteLine("\n");
            Console.WriteLine("-Create Account-");
            Console.WriteLine("Person Id: " + account.PersonId);
            Console.WriteLine("CNP: " + account.Cnp);
            Console.WriteLine("Balance: " + account.Balance);
            Console.WriteLine("Account Type: " + account.AccountType);
            Console.WriteLine("Currency: " + account.Currency);
            Console.WriteLine("Status account: " + account.Status);
            Console.WriteLine("IBAN: " + account.IBanCode);

            CreateAccount acc = new CreateAccount(eventSender);
            acc.PerformOperation(account, db);







          
            DepositMoneyCommand deposit = new DepositMoneyCommand();
           // deposit.AccountId = 0;
            deposit.Name = "Ilie Andrei";
            deposit.Currency = "RON";
            deposit.Value = 10000;
            deposit.Iban = "RO12INGB1234567890";

            Console.WriteLine("\n");
            Console.WriteLine("-Deposit Money-");
            Console.WriteLine("Name: " + deposit.Name);
            Console.WriteLine("Currency: " + deposit.Currency);
            Console.WriteLine("IBAN: " + deposit.Iban);
            Console.WriteLine("Value: " + deposit.Value);

            DepositMoney dep = new DepositMoney(eventSender);
            dep.PerformOperation(deposit, db);



            WithdrawMoneyCommand withdraw = new WithdrawMoneyCommand();
            withdraw.Name = "Ilie Andrei";
            withdraw.Currency = "RON";
            withdraw.Value = 99;
            withdraw.Iban = "RO12INGB1234567890";

            Console.WriteLine("\n");
            Console.WriteLine("-Withdraw Customer-");
            Console.WriteLine("Name: " + withdraw.Name);
            Console.WriteLine("Currency: " + withdraw.Currency);
            Console.WriteLine("IBAN: " + withdraw.Iban);
            Console.WriteLine("Value: " + withdraw.Value);

            WithdrawMoney wit = new WithdrawMoney(eventSender);
            wit.PerformOperation(withdraw, db);


           


            CreateProductCommand prod1cmd = new CreateProductCommand();

            prod1cmd.ProductId = 1;
            prod1cmd.Name = "carte";
            prod1cmd.Value = 10;
            prod1cmd.Currency = "RON";
            prod1cmd.Limit = 50;

            CreateProduct p1 = new CreateProduct(eventSender);
            p1.PerformOperation(prod1cmd, db);

   



            CreateProductCommand prod2cmd = new CreateProductCommand();
            prod2cmd.ProductId = 2;
            prod2cmd.Name = "caiet";
            prod2cmd.Value = 5;
            prod2cmd.Currency = "RON";
            prod2cmd.Limit = 70;

            CreateProduct p2 = new CreateProduct(eventSender);
            p2.PerformOperation(prod2cmd, db);




            
            PurchaseProductCommand purchase = new PurchaseProductCommand();
            purchase.Iban = "RO12INGB1234567890";
            purchase.Name= "Ilie Andrei";
            purchase.Data = DateTime.UtcNow;
            purchase.ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = prod1cmd.ProductId, Quantity = 3 },
                new PurchaseProductDetail { ProductId = prod2cmd.ProductId, Quantity = 4 }
            };


            PurchaseProduct purchaseProd = new PurchaseProduct(eventSender);
            purchaseProd.PerformOperation(purchase, db);

        }
    }
}
