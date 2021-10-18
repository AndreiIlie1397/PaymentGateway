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

            var iban = Guid.NewGuid().ToString();
            var cnp = Guid.NewGuid().ToString().Substring(0, 13);

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
                Currency = "RON"
            };

            await mediator.Send(command, cancellationToken);

            CreateAccountCommand account = new()
            {
                PersonId = 1,
                Currency = "RON",
                Cnp = cnp,
                Type = "Current",
                Iban = iban
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

            await mediator.Send(account, cancellationToken);

            DepositMoneyCommand deposit = new()
            {
                Currency = "RON",
                Amount = 101,
                Iban = iban,
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            await mediator.Send(deposit, cancellationToken);

            WithdrawMoneyCommand withdraw = new()
            {
                Currency = "RON",
                Amount = 99,
                Iban = iban,
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now
            };

            await mediator.Send(withdraw, cancellationToken);

            CreateProductCommand prod1cmd = new()
            {
                Name = "pix",
                Value = 2,
                Currency = "RON",
                Limit = 100
            };

            await mediator.Send(prod1cmd, cancellationToken);

            CreateProductCommand prod2cmd = new()
            {
                Name = "creion",
                Value = 1,
                Currency = "RON",
                Limit = 100
            };

            await mediator.Send(prod2cmd, cancellationToken);

            var listProducts = new List<PurchaseProductDetail>();
            var prodCmd1 = new PurchaseProductDetail
            {
                TransactionId = 3,
                ProductId = 3,
                Quantity = 3
            };

           listProducts.Add(prodCmd1);

            var prodCmd2 = new PurchaseProductDetail
            {
                TransactionId = 3,
                ProductId = 4,
                Quantity = 4
            };

           listProducts.Add(prodCmd2);

            PurchaseProductCommand purchase = new()
            {
                Iban = iban,
                Currency = "RON",
                DateOfTransaction = DateTime.Now,
                DateOfOperation = DateTime.Now,
                ProductDetails = new List<PurchaseProductDetail>
            {
                new PurchaseProductDetail { ProductId = prodCmd1.ProductId, TransactionId=prodCmd1.TransactionId, Quantity = 3 },
                new PurchaseProductDetail { ProductId = prodCmd2.ProductId, TransactionId=prodCmd2.TransactionId, Quantity = 4 }
            }
            };

            await mediator.Send(purchase, cancellationToken);

            var query = new ListOfAccounts.Query
            {
                PersonId = 1
            };

            var result = await mediator.Send(query, cancellationToken);

            database.SaveChanges();
        }
    }
}
