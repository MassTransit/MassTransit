namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Linq;
    using Configuration;


    public class QueueErrorSettings :
        AmazonSqsQueueSubscriptionConfigurator,
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
