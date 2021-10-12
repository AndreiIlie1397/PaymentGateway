using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Abstractions;
using PaymentGateway.Application;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.ExternalService;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.IO;
using static PaymentGateway.PublishedLanguage.WriteSide.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static void Main(string[] args)
        {
            Database db = new Database();

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // setup
            var services = new ServiceCollection();
            services.RegisterBusinessServices(Configuration);

            services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();

            // use
            EnrollCustomerCommand command = new EnrollCustomerCommand
            {
                Name = "Ilie Andrei",
                UniqueIdentifier = "1234567890",
                ClientType = "Individual",
                AccountType = "Current",
                Currency = "RON"
            };


            var client = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            client.PerformOperation(command, db);







            CreateAccountCommand account = new CreateAccountCommand
            {
                //account.IBanCode = "RO12INGB132435678";
                PersonId = 0,
                Currency = "RON",
                Cnp = "197231456445",
                AccountType = "Current",
                IBanCode = "RO12INGB1234567890"
            };

            Console.WriteLine("\n");
            Console.WriteLine("-Create Account-");
            Console.WriteLine("Person Id: " + account.PersonId);
            Console.WriteLine("CNP: " + account.Cnp);
            Console.WriteLine("Balance: " + account.Balance);
            Console.WriteLine("Account Type: " + account.AccountType);
            Console.WriteLine("Currency: " + account.Currency);
            Console.WriteLine("Status account: " + account.Status);
            Console.WriteLine("IBAN: " + account.IBanCode);

            services.AddSingleton<AccountOptions>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new AccountOptions
                {
                    InitialBalance = config.GetValue("AccountOptions:InitialBalance", 0)
                };
                return options;
            });


            CreateAccount acc = serviceProvider.GetRequiredService<CreateAccount>();
            acc.PerformOperation(account, db);








            DepositMoneyCommand deposit = new DepositMoneyCommand
            {
                // deposit.AccountId = 0;
                Name = "Ilie Andrei",
                Currency = "RON",
                Value = 10000,
                Iban = "RO12INGB1234567890"
            };

            Console.WriteLine("\n");
            Console.WriteLine("-Deposit Money-");
            Console.WriteLine("Name: " + deposit.Name);
            Console.WriteLine("Currency: " + deposit.Currency);
            Console.WriteLine("IBAN: " + deposit.Iban);
            Console.WriteLine("Value: " + deposit.Value);

            DepositMoney dep = serviceProvider.GetRequiredService<DepositMoney>();
            dep.PerformOperation(deposit, db);



            WithdrawMoneyCommand withdraw = new WithdrawMoneyCommand
            {
                Name = "Ilie Andrei",
                Currency = "RON",
                Value = 99,
                Iban = "RO12INGB1234567890"
            };

            Console.WriteLine("\n");
            Console.WriteLine("-Withdraw Customer-");
            Console.WriteLine("Name: " + withdraw.Name);
            Console.WriteLine("Currency: " + withdraw.Currency);
            Console.WriteLine("IBAN: " + withdraw.Iban);
            Console.WriteLine("Value: " + withdraw.Value);

            WithdrawMoney wit = serviceProvider.GetRequiredService<WithdrawMoney>();
            wit.PerformOperation(withdraw, db);





            CreateProductCommand prod1cmd = new CreateProductCommand
            {
                ProductId = 1,
                Name = "carte",
                Value = 10,
                Currency = "RON",
                Limit = 50
            };

            CreateProduct p1 = serviceProvider.GetRequiredService<CreateProduct>();
            p1.PerformOperation(prod1cmd, db);





            CreateProductCommand prod2cmd = new CreateProductCommand
            {
                ProductId = 2,
                Name = "caiet",
                Value = 5,
                Currency = "RON",
                Limit = 70
            };

            CreateProduct p2 = serviceProvider.GetRequiredService<CreateProduct>();
            p2.PerformOperation(prod2cmd, db);





            PurchaseProductCommand purchase = new PurchaseProductCommand();
            purchase.Iban = "RO12INGB1234567890";
            purchase.Name = "Ilie Andrei";
            purchase.Data = DateTime.UtcNow;
            purchase.ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = prod1cmd.ProductId, Quantity = 3 },
                new PurchaseProductDetail { ProductId = prod2cmd.ProductId, Quantity = 4 }
            };


            PurchaseProduct purchaseProd = serviceProvider.GetRequiredService<PurchaseProduct>();
            purchaseProd.PerformOperation(purchase, db);

        }
    }
}
