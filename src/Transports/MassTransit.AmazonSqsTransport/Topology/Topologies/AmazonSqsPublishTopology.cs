namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;


    public class AmazonSqsPublishTopology :
        PublishTopology,
        IAmazonSqsPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public AmazonSqsPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IAmazonSqsMessagePublishTopology<T> IAmazonSqsPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IAmazonSqsMessagePublishTopology<T>;
        }

        IAmazonSqsMessagePublishTopologyConfigurator<T> IAmazonSqsPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IAmazonSqsMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
