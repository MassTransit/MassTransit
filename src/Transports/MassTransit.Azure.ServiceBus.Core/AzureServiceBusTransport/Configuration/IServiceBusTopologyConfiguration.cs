namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IServiceBusTopologyConfiguration :
        ITopologyConfiguration
    {
        new IServiceBusPublishTopologyConfigurator Publish { get; }

        new IServiceBusSendTopologyConfigurator Send { get; }

        new IServiceBusConsumeTopologyConfigurator Consume { get; }
    }
}
