using PaymentGateway.Data;
using PaymentGateway.Models;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.Application.Services
{
    public class NewIban
    {
        private readonly PaymentDbContext _dbContext;

        public NewIban(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetNewIban()
        {
            List<string> ibans = _dbContext.Accounts.Select(x => x.Iban).ToList();

            if (ibans.Count == 0)
                return "1";

            return (long.Parse(ibans.Last()) + 1).ToString();
        }
    }
}
