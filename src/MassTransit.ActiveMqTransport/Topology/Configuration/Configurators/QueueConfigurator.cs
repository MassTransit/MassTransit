namespace MassTransit.ActiveMqTransport.Topology.Configurators
{
    using Entities;


    public class QueueConfigurator :
        EntityConfigurator,
        IQueueConfigurator,
        Queue
    {
        public QueueConfigurator(string queueName, bool durable = true, bool autoDelete = false)
            : base(queueName, durable, autoDelete)
        {
        }

        public QueueConfigurator(Queue source)
            : base(source.EntityName, source.Durable, source.AutoDelete)
        {
        }

        public bool Lazy { get; set; }

        protected override ActiveMqEndpointAddress.AddressType AddressType => ActiveMqEndpointAddress.AddressType.Queue;
    }
}
