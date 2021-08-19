namespace MassTransit.AmazonSqsTransport.Topology.Settings
{
    using System.Linq;
    using Builders;
    using Configurators;


    public class QueueErrorSettings :
        QueueSubscriptionConfigurator,
        ErrorSettings
    {
        public QueueErrorSettings(ReceiveSettings source, string queueName)
            : base(queueName, source.Durable, source.AutoDelete)
        {
            QueueTags = source.Tags?.ToDictionary(x => x.Key, x => x.Value);
            QueueAttributes = source.QueueAttributes?.ToDictionary(x => x.Key, x => x.Value);
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.CreateQueue(EntityName, Durable, AutoDelete, QueueAttributes, QueueSubscriptionAttributes, QueueTags);

            return builder.BuildBrokerTopology();
        }
    }
}
