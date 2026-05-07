using Topup.Domain.Enums;

namespace Topup.Domain.Entities
{
    public class TopupSaga
    {
        public Guid Id { get; set; }

        public Guid TransactionId { get; set; }

        public SagaState State { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
