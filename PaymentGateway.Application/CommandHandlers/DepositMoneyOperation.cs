using System;
using PaymentGateway.PublishedLanguage.Commands;
using System.Linq;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Events;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class DepositMoneyOperation : IRequestHandler<DepositMoneyCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;
        public DepositMoneyOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(DepositMoneyCommand request, CancellationToken cancellationToken)
        {
            //Database database = Database.GetInstance();
            Account account;

            if (request.AccountId.HasValue)
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

            if (request.Amount <= 0)
            {
                throw new Exception("Cannot deposit negative amount");
            }

            if (!String.IsNullOrEmpty(request.Currency))
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }

            Transaction transaction = new()
            {
                Currency = request.Currency,
                Amount = request.Amount,
                Type = (int)TransactionType.Deposit,
                Date = request.DateOfTransaction,
                AccountId = account.Id
            };

            account.Balance += request.Amount;

            _dbContext.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            await _mediator.Publish(transactionCreated, cancellationToken);

            DepositCreated depositCreated = new(request.Iban, account.Balance, account.Currency);
            await _mediator.Publish(depositCreated, cancellationToken);

            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
