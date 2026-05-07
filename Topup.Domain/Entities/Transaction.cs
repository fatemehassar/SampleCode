using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Topup.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }

        public decimal Amount { get; private set; }

        public string MobileNo { get; private set; }

        public string IdempotencyKey { get; private set; }

        public TransactionStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private Transaction()
        {
        }

        public Transaction(
            decimal amount,
            string mobileNo,
            string idempotencyKey)
        {
            Id = Guid.NewGuid();

            Amount = amount;

            MobileNo = mobileNo;

            IdempotencyKey = idempotencyKey;

            Status = TransactionStatus.PendingPurchase;

            CreatedAt = DateTime.UtcNow;
        }

        public void MarkCompleted()
        {
            Status = TransactionStatus.Completed;
        }

        public void MarkReversed()
        {
            Status = TransactionStatus.Reversed;
        }

        public void MarkProcessing()
        {
            Status = TransactionStatus.TopupProcessing;
        }
    }
}
