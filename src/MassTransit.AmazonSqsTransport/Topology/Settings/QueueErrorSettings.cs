namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System.Linq;
    using Builders;
    using Configurators;


    public class QueueErrorSettings :
        QueueSubscriptionConfigurator,
        ErrorSettings
    {
        public QueueErrorSettings(EntitySettings source, string queueName)
            : base(queueName, source.Durable, source.AutoDelete)
        {
            QueueTags = source.Tags?.ToDictionary(x => x.Key, x => x.Value);
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.CreateQueue(EntityName, Durable, AutoDelete, null, null, QueueTags);

            return builder.BuildBrokerTopology();
        }
    }
}
