namespace MassTransit.ActiveMqTransport.Topology
{
    public interface IActiveMqTemporaryQueueNameFormatter
    {
        public string Format(string queueName);
    }
}
