namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using Builders;
    using Configurators;


    public class QueueDeadLetterSettings :
        QueueSubscriptionConfigurator,
        DeadLetterSettings
    {
        public QueueDeadLetterSettings(EntitySettings source, string queueName)
            : base(queueName, source.Durable, source.AutoDelete)
        {
            QueueTags = source.Tags;
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.CreateQueue(EntityName, Durable, AutoDelete, null, null, QueueTags);

            return builder.BuildBrokerTopology();
        }
    }
}
