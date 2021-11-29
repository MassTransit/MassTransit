namespace MassTransit.ActiveMqTransport.Configuration
{
    using Topology;


    public class ActiveMqQueueConfigurator :
        EntityConfigurator,
        IActiveMqQueueConfigurator,
        Queue
    {
        protected ActiveMqQueueConfigurator(string queueName, bool durable = true, bool autoDelete = false)
            : base(queueName, durable, autoDelete)
        {
        }

        protected override ActiveMqEndpointAddress.AddressType AddressType => ActiveMqEndpointAddress.AddressType.Queue;
    }
}
