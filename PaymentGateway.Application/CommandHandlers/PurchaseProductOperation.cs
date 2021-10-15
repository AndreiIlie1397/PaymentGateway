using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using PaymentGateway.PublishedLanguage.Commands;
using System;
using System.Linq;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class PurchaseProductOperation : IRequestHandler<PurchaseProductCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;

        public PurchaseProductOperation(IMediator mediator, Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            double totalAmount = 0.0;

            Account account;
            Product product;

            if (request.PersonId.HasValue)
            {
                account = _database.Accounts.FirstOrDefault(x => x.Id == request.AccountId);
            }
            else
            {
                account = _database.Accounts.FirstOrDefault(x => x.IbanCode == request.Iban);
            }

            if (account == null)
            {
                throw new Exception("Account not found");
            }

            foreach (var item in request.ProductDetails)
            {
                product = _database.Products.FirstOrDefault(x => x.Id == item.ProductId);

                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Insufficient stocks!");
                }
                product.Limit -= item.Quantity;

                totalAmount += item.Quantity * product.Value;
            }

            if (account.Balance < totalAmount)
            {
                throw new Exception("You have insufficient funds!");
            }

            Transaction transaction = new();
            transaction.Amount = -totalAmount;
            _database.Transactions.Add(transaction);
            account.Balance -= totalAmount;

            foreach (var item in request.ProductDetails)
            {
                product = _database.Products.FirstOrDefault(x => x.Id == item.ProductId);
                ProductXTransaction productXTransaction = new()
                {
                    TransactionId = transaction.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Value = product.Value,
                    Name = product.Name
                };
            }

            ProductPurchased eventProductPurchased = new() { ProductDetails = request.ProductDetails };
            await _mediator.Publish(eventProductPurchased, cancellationToken);

            Database.SaveChanges();
            return Unit.Value;
        }
    }
}