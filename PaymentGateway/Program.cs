using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
using PaymentGateway.Application.Services;
using PaymentGateway.Data;
using PaymentGateway.ExternalService;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static PaymentGateway.PublishedLanguage.Commands.PurchaseProductCommand;

namespace PaymentGateway
{
    class Program
    {
        static IConfiguration Configuration;
        static async Task Main(string[] args)
        {

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // setup
            var services = new ServiceCollection();

            var source = new CancellationTokenSource();
            var cancellationToken = source.Token;
            services.RegisterBusinessServices(Configuration);
            services.AddPaymentDataAccess(Configuration);

            services.Scan(scan => scan
                   .FromAssemblyOf<ListOfAccounts>()
                   .AddClasses(classes => classes.AssignableTo<IValidator>())
                   .AsImplementedInterfaces()
                   .WithScopedLifetime());

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

            services.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));

            services.AddScopedContravariant<INotificationHandler<INotification>, AllEventsHandler>(typeof(EnrollCustomerCommand).Assembly);

            services.AddMediatR(new[] { typeof(ListOfAccounts).Assembly, typeof(AllEventsHandler).Assembly }); // get all IRequestHandler and INotificationHandler classes

            services.AddSingleton(Configuration);

            // build
            var serviceProvider = services.BuildServiceProvider();
            var database = serviceProvider.GetRequiredService<PaymentDbContext>();
            var ibanService = serviceProvider.GetRequiredService<NewIban>();
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            //use
           EnrollCustomerCommand command = new()
           {
               Name = "Ilie Andrei",
               UniqueIdentifier = Guid.NewGuid().ToString(),
               ClientType = "Individual",
               //AccountType = "Current"
               Currency = "RON"
           };

            //var client = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            await mediator.Send(command, cancellationToken);

            CreateAccountCommand account = new()
            {
                PersonId = 1,
                Currency = "RON",
                Cnp = Guid.NewGuid().ToString(),
                Type = "Current",
                Iban = (Int64.Parse(ibanService.GetNewIban()) - 1).ToString()
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
            await mediator.Send(account, cancellationToken);

            DepositMoneyCommand deposit = new()
            {
                // deposit.AccountId = 0;
                Currency = "RON",
                Amount = 1001,
                Iban = (Int64.Parse(ibanService.GetNewIban()) - 1).ToString(),
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            //DepositMoneyOperation dep = serviceProvider.GetRequiredService<DepositMoneyOperation>();
           await mediator.Send(deposit, cancellationToken);

            WithdrawMoneyCommand withdraw = new()
            {
                Name = "Ilie Andrei",
                Currency = "RON",
                Amount = 99,
                Iban = (Int64.Parse(ibanService.GetNewIban()) - 1).ToString(),
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            //WithdrawMoneyOperation wit = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            await mediator.Send(withdraw, cancellationToken);

            CreateProductCommand prod1cmd = new()
            {
                Name = "carte",
                Value = 10,
                Currency = "RON",
                Limit = 100
            };

            //CreateProductOperation p1 = serviceProvider.GetRequiredService<CreateProductOperation>();
                await mediator.Send(prod1cmd, cancellationToken);

            CreateProductCommand prod2cmd = new()
            {
                Name = "caiet",
                Value = 5,
                Currency = "RON",
                Limit = 100
            };

            //CreateProductOperation p2 = serviceProvider.GetRequiredService<CreateProductOperation>();
            await mediator.Send(prod2cmd, cancellationToken);

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

            PurchaseProductCommand purchase = new()
            {
                Iban = (Int64.Parse(ibanService.GetNewIban()) - 1).ToString(),
                Currency = "RON",
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now,
                ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = prodCmd1.ProductId,  Quantity = 3 },
                new PurchaseProductDetail { ProductId = prodCmd2.ProductId, Quantity = 4 }
            }
            };

            //PurchaseProductOperation purchaseProd = serviceProvider.GetRequiredService<PurchaseProductOperation>();
            await mediator.Send(purchase, cancellationToken);

            var query = new ListOfAccounts.Query
            {
                PersonId = 1
            };
            //var handler = serviceProvider.GetRequiredService<ListOfAccounts.QueryHandler>();
            var result = await mediator.Send(query, cancellationToken);

            database.SaveChanges();
        }
    }
}
