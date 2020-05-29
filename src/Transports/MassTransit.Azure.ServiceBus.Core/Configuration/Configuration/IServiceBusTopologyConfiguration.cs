namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IServiceBusTopologyConfiguration :
        ITopologyConfiguration
    {
        new IServiceBusPublishTopologyConfigurator Publish { get; }

        new IServiceBusSendTopologyConfigurator Send { get; }

        new IServiceBusConsumeTopologyConfigurator Consume { get; }
    }
}
