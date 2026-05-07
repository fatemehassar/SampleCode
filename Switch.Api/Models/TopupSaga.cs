using Switch.Api.Enums;

namespace Switch.Api.Models
{
    public class TopupSaga
    {
        public Guid Id { get; set; }

        public Guid TransactionId { get; set; }

        public SagaState State { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
