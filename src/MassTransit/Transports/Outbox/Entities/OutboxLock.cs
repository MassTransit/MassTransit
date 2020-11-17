namespace MassTransit.Transports.Outbox.Entities
{
    public class OutboxLock
    {
        public string OutboxName { get; set; }
        public string LockName { get; set; }
    }
}
