using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application;
using PaymentGateway.Application.Queries;
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
            Database database = new();

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

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            // use
            EnrollCustomerCommand command = new()
            {
                Name = "Ilie Andrei",
                UniqueIdentifier = "1234567890",
                ClientType = "Individual",
                AccountType = "Current",
                Currency = "RON"
            };

            //var client = serviceProvider.GetRequiredService<EnrollCustomerOperation>();
            await mediator.Send(command, cancellationToken);

            CreateAccountCommand account = new()
            {
                //account.IBanCode = "RO12INGB132435678";
                PersonId = 1,
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
            await mediator.Send(account, cancellationToken);

            DepositMoneyCommand deposit = new()
            {
                // deposit.AccountId = 0;
                Currency = "RON",
                Value = 10000,
                Iban = "RO12INGB1234567890"
            };

            //DepositMoneyOperation dep = serviceProvider.GetRequiredService<DepositMoneyOperation>();
           await mediator.Send(deposit, cancellationToken);

            WithdrawMoneyCommand withdraw = new()
            {
                Name = "Ilie Andrei",
                Currency = "RON",
                Value = 99,
                Iban = "RO12INGB1234567890"
            };

            //WithdrawMoneyOperation wit = serviceProvider.GetRequiredService<WithdrawMoneyOperation>();
            await mediator.Send(withdraw, cancellationToken);

            CreateProductCommand prod1cmd = new()
            {
                ProductId = 1,
                Name = "carte",
                Value = 10,
                Currency = "RON",
                Limit = 50
            };

            //CreateProductOperation p1 = serviceProvider.GetRequiredService<CreateProductOperation>();
                await mediator.Send(prod1cmd, cancellationToken);

            CreateProductCommand prod2cmd = new()
            {
                ProductId = 2,
                Name = "caiet",
                Value = 5,
                Currency = "RON",
                Limit = 70
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
                Iban = "RO12INGB1234567890",
                ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = prodCmd1.ProductId, Quantity = 3 },
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
        }
    }
}
