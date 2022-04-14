namespace MassTransit.Tests.ReliableMessaging
{
    public class Command
    {
        public bool FailWhenConsuming { get; set; }
        public int? FailAfterProducing { get; set; }
    }
}
