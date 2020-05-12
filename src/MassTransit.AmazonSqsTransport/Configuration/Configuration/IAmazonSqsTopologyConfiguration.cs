namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IAmazonSqsTopologyConfiguration :
        ITopologyConfiguration
    {
        new IAmazonSqsPublishTopologyConfigurator Publish { get; }

        new IAmazonSqsSendTopologyConfigurator Send { get; }

        new IAmazonSqsConsumeTopologyConfigurator Consume { get; }
    }
}
