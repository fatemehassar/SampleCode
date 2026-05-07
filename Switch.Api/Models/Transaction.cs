using TransactionStatus = Switch.Api.Enums.TransactionStatus;

namespace Switch.Api.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public string MobileNo { get; set; }

        public string IdempotencyKey { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
