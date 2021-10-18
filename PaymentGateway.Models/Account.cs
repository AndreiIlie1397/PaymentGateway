using System;
using System.Collections.Generic;

#nullable disable

namespace PaymentGateway.Models
{
    public partial class Account
    {
        public Account()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string Iban { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public decimal? Limit { get; set; }
        public int? PersonId { get; set; }

        public virtual Person Person { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
