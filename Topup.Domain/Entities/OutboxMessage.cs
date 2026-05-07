namespace Topup.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Payload { get; set; }
        public bool Processed { get; set; }
    }
}
