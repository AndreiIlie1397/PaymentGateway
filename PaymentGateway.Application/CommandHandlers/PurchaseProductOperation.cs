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
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;

        public PurchaseProductOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            decimal totalAmount = 0;

            Account account;
            Product product;

            if (request.PersonId.HasValue)
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.Id == request.AccountId);
            }
            else
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.Iban == request.Iban);
            }
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            foreach (var item in request.ProductDetails)
            {
                product = _dbContext.Products.FirstOrDefault(x => x.Id == item.ProductId);

                if (item.Quantity < 1)
                {
                    throw new Exception("Quantity is negative");
                }
                if (product.Limit < item.Quantity)
                {
                    throw new Exception("Product not in stock");
                }

                totalAmount += item.Quantity * product.Value;

                if (account.Balance < totalAmount)
                {
                    throw new Exception("You have insufficient funds!");
                }

                //ProductXTransaction productXTransaction = new()
                //{
                //    TransactionId = item.TransactionId,
                //    ProductId = item.ProductId,
                //    Quantity = item.Quantity
                //};

                //_dbContext.ProductXTransactions.Add(productXTransaction);

                
                account.Balance -= totalAmount;
                product.Limit -= item.Quantity;
  
            }
            Transaction transaction = new()
            {
                Currency = request.Currency,
                Amount = -totalAmount,
                Type = TransactionType.PurchaseService,
                Date = request.DateOfTransaction,
                AccountId = account.Id
            };

            _dbContext.Transactions.Add(transaction);

            ProductPurchased eventProductPurchased = new() { ProductDetails = request.ProductDetails };
            await _mediator.Publish(eventProductPurchased, cancellationToken);

            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}