namespace Switch.Api.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Payload { get; set; }
        public bool Processed { get; set; }
    }
}
