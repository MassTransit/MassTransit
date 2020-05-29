namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IRabbitMqTopologyConfiguration :
        ITopologyConfiguration
    {
        new IRabbitMqPublishTopologyConfigurator Publish { get; }

        new IRabbitMqSendTopologyConfigurator Send { get; }

        new IRabbitMqConsumeTopologyConfigurator Consume { get; }
    }
}
