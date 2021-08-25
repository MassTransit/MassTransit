namespace MassTransit.Transports.Outbox.Entities
{
    public class OnRampLock
    {
        public string OnRampName { get; set; }
        public string LockName { get; set; }
    }
}
