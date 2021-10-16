namespace PaymentGateway.Models
{
    public class Account
    {
        public int? Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string Iban { get; set; }
        public AccountType Type { get; set; }
        public AccountStatus Status { get; set; }
        public decimal Limit { get; set; }

        public int? PersonId { get; set; }
    }
}
