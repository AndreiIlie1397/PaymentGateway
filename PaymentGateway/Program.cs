using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Abstractions;
using PaymentGateway.Application;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.ExternalService;
using PaymentGateway.Models;
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
            Database database = new Database();

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
            client.PerformOperation(command);







            CreateAccountCommand account = new CreateAccountCommand
            {
                //account.IBanCode = "RO12INGB132435678";
                PersonId = 0,
                Currency = "RON",
                Cnp = "197231456445",
                Type = "Current",
                IBanCode = "RO12INGB1234567890"
            };

            Console.WriteLine("\n");
            Console.WriteLine("-Create Account-");
            Console.WriteLine("Person Id: " + account.PersonId);
            Console.WriteLine("CNP: " + account.Cnp);
            Console.WriteLine("Balance: " + account.Balance);
            Console.WriteLine("Account Type: " + account.Type);
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


            CreateAccountOperation acc = serviceProvider.GetRequiredService<CreateAccountOperation>();
            acc.PerformOperation(account);








            DepositMoneyCommand deposit = new DepositMoneyCommand
            {
                // deposit.AccountId = 0;
                Currency = "RON",
                Value = 10000,
                Iban = "RO12INGB1234567890"
            };

            Console.WriteLine("\n");
            Console.WriteLine("-Deposit Money-");
            Console.WriteLine("Currency: " + deposit.Currency);
            Console.WriteLine("IBAN: " + deposit.Iban);
            Console.WriteLine("Value: " + deposit.Value);

            DepositMoneyOperation dep = serviceProvider.GetRequiredService<DepositMoneyOperation>();
            dep.PerformOperation(deposit);



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

            WithdrawMoneyOperation wit = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            wit.PerformOperation(withdraw);





            CreateProductCommand prod1cmd = new CreateProductCommand
            {
                ProductId = 1,
                Name = "carte",
                Value = 10,
                Currency = "RON",
                Limit = 50
            };


            CreateProductOperation p1 = serviceProvider.GetRequiredService<CreateProductOperation>();
            p1.PerformOperation(prod1cmd);

            CreateProductCommand prod2cmd = new CreateProductCommand
            {
                ProductId = 2,
                Name = "caiet",
                Value = 5,
                Currency = "RON",
                Limit = 70
            };


            //db.Products.Add(prod1cmd);
            //db.Products.Add(prod2cmd);

            CreateProductOperation p2 = serviceProvider.GetRequiredService<CreateProductOperation>();
            p2.PerformOperation(prod2cmd);





            //Product product1 = new Product
            //{
            //    Id = 1,
            //    Name = "carte",
            //    Value = 10,
            //    Currency = "RON",
            //    Limit = 1000
            //};

            //Product product2 = new Product
            //{
            //    Id = 2,
            //    Name = "caiet",
            //    Value = 5,
            //    Currency = "RON",
            //    Limit = 1000
            //};

            //database.Products.Add(product1);
            //database.Products.Add(product2);







            var listProducts = new List<PurchaseProductDetail>();
            var prodCmd1 = new PurchaseProductDetail
            {
                ProductId = 1,
                Quantity = 3
        };

            listProducts.Add(prodCmd1);

            var prodCmd2 = new PurchaseProductDetail
            {
                ProductId = 2,
                Quantity = 3
            };

            listProducts.Add(prodCmd2);


            PurchaseProductCommand purchase = new PurchaseProductCommand();
            purchase.Iban = "RO12INGB1234567890";
            purchase.ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = prodCmd1.ProductId, Quantity = 3 },
                new PurchaseProductDetail { ProductId = prodCmd2.ProductId, Quantity = 4 }
            };


            PurchaseProductOperation purchaseProd = serviceProvider.GetRequiredService<PurchaseProductOperation>();
            purchaseProd.PerformOperation(purchase);

        }
    }
}
