
namespace PaymentGateway.Models
{
    public class ProductXTransaction
    {
        public int? ProductId { get; set; }
        public int? TransactionId { get; set; }
        public decimal Quantity { get; set; }
    }
}
