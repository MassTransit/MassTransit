namespace MassTransit.ActiveMqTransport.Topology.Configurators
{
    using Entities;


    public class QueueConfigurator :
        EntityConfigurator,
        IQueueConfigurator,
        Queue
    {
        protected QueueConfigurator(string queueName, bool durable = true, bool autoDelete = false)
            : base(queueName, durable, autoDelete)
        {
        }

        protected override ActiveMqEndpointAddress.AddressType AddressType => ActiveMqEndpointAddress.AddressType.Queue;
    }
}
