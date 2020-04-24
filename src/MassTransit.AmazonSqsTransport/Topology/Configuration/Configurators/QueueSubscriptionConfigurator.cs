namespace MassTransit.AmazonSqsTransport.Topology.Configurators
{
    using Configuration;


    public class QueueSubscriptionConfigurator :
        QueueConfigurator,
        IQueueSubscriptionConfigurator
    {
        protected QueueSubscriptionConfigurator(string queueName, bool durable, bool autoDelete)
            : base(queueName, durable, autoDelete)
        {
        }
    }
}
