using MediatR;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.PublishedLanguage.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.CommandHandlers
{
    public class WithdrawMoneyOperation : IRequestHandler<WithdrawMoneyCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;
        public WithdrawMoneyOperation(IMediator mediator, PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(WithdrawMoneyCommand request, CancellationToken cancellationToken)
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

            if (account.Balance < request.Amount)
            {
                throw new Exception("Cannot withdraw money");
            }

            if (!String.IsNullOrEmpty(request.Currency))
            {
                account = _dbContext.Accounts.FirstOrDefault(x => x.Currency == request.Currency);
            }

            Transaction transaction = new()
            {
                Currency = request.Currency,
                Amount = -request.Amount,
                Type = (int)TransactionType.Withdraw,
                Date = request.DateOfTransaction,
                AccountId = account.Id
            };

            account.Balance -= request.Amount;

            _dbContext.Transactions.Add(transaction);

            TransactionCreated transactionCreated = new(request.Amount, request.Currency, request.DateOfTransaction);
            await _mediator.Publish(transactionCreated, cancellationToken);

            WithdrawCreated withdrawCreated = new(account.Iban, account.Balance, account.Currency);
            await _mediator.Publish(withdrawCreated, cancellationToken);

            _dbContext.SaveChanges();
            return Unit.Value;
        }
    }
}
