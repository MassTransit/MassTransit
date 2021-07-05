namespace MassTransit.ActiveMqTransport.Topology
{
    public interface IActiveMqConsumerEndpointQueueNameFormatter
    {
        public string Format(string topic, string endpointName);
    }
}
