using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Abstractions;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.ExternalService;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using static PaymentGateway.PublishedLanguage.Commands.PurchaseProductCommand;

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

            services.AddMediatR(typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly);

            services.RegisterBusinessServices(Configuration);

            //services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<MediatR.IMediator>();

            // use
            EnrollCustomerCommand command = new EnrollCustomerCommand
            {
                Name = "Ilie Andrei",
                UniqueIdentifier = "1234567890",
                ClientType = "Individual",
                AccountType = "Current",
                Currency = "RON"
            };


            //var client = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            mediator.Send(command, default).GetAwaiter().GetResult();







            CreateAccountCommand account = new CreateAccountCommand
            {
                //account.IBanCode = "RO12INGB132435678";
                PersonId = 0,
                Currency = "RON",
                Cnp = "197231456445",
                Type = "Current",
                IBanCode = "RO12INGB1234567890"
            };

            services.AddSingleton<AccountOptions>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var options = new AccountOptions
                {
                    InitialBalance = config.GetValue("AccountOptions:InitialBalance", 0)
                };
                return options;
            });


            // CreateAccountOperation acc = serviceProvider.GetRequiredService<CreateAccountOperation>();
            mediator.Send(account, default).GetAwaiter().GetResult();








            DepositMoneyCommand deposit = new DepositMoneyCommand
            {
                // deposit.AccountId = 0;
                Currency = "RON",
                Value = 10000,
                Iban = "RO12INGB1234567890"
            };

            //DepositMoneyOperation dep = serviceProvider.GetRequiredService<DepositMoneyOperation>();
            mediator.Send(deposit, default).GetAwaiter().GetResult();



            WithdrawMoneyCommand withdraw = new WithdrawMoneyCommand
            {
                Name = "Ilie Andrei",
                Currency = "RON",
                Value = 99,
                Iban = "RO12INGB1234567890"
            };

            //WithdrawMoneyOperation wit = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            mediator.Send(withdraw, default).GetAwaiter().GetResult();





            CreateProductCommand prod1cmd = new CreateProductCommand
            {
                ProductId = 1,
                Name = "carte",
                Value = 10,
                Currency = "RON",
                Limit = 50
            };


            //CreateProductOperation p1 = serviceProvider.GetRequiredService<CreateProductOperation>();
            mediator.Send(prod1cmd, default).GetAwaiter().GetResult();

            CreateProductCommand prod2cmd = new CreateProductCommand
            {
                ProductId = 2,
                Name = "caiet",
                Value = 5,
                Currency = "RON",
                Limit = 70
            };

            //CreateProductOperation p2 = serviceProvider.GetRequiredService<CreateProductOperation>();
            mediator.Send(prod2cmd, default).GetAwaiter().GetResult();

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


            //PurchaseProductOperation purchaseProd = serviceProvider.GetRequiredService<PurchaseProductOperation>();
            mediator.Send(purchase, default).GetAwaiter().GetResult();




            var query = new ListOfAccounts.Query
            {
                PersonId = 1
            };
            var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            var result = handler.Handle(query, default).GetAwaiter().GetResult();
        }
    }
}
