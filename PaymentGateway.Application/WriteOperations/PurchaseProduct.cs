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
    public class PurchaseProduct : IWriteOperation<PurchaseProductCommand>
    {
        public IEventSender eventSender;

        public PurchaseProduct(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }

        public void PerformOperation(PurchaseProductCommand operation, Database database)
        {
            double totalAmount = 0.0;

            Account account = database.GetAccountByIban(operation.Iban);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            foreach (var item in operation.ProductDetails)
            {

                var product = database.Product.FirstOrDefault(x => x.Id == item.ProductId);

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

            Transaction transaction = new Transaction();
            transaction.Amount = -totalAmount;
            database.Transactions.Add(transaction);
            account.Balance -= totalAmount;

            foreach (var item in operation.ProductDetails)
            {
                var product = database.Product.FirstOrDefault(x => x.Id == item.ProductId);
                ProductXTransaction productXTransaction = new ProductXTransaction();
                productXTransaction.TransactionId = transaction.Id;
                productXTransaction.ProductId = item.ProductId;
                productXTransaction.Quantity = item.Quantity;
                productXTransaction.Value = product.Value;
                productXTransaction.Name = product.Name;
            }

            ProductPurchased eventProductPurchased = new ProductPurchased { ProductDetails = operation.ProductDetails };
            eventSender.SendEvent(eventProductPurchased);
            database.SaveChanges();







            database.Transactions.Add(transaction);
            database.SaveChanges();


         
        }
    }
}