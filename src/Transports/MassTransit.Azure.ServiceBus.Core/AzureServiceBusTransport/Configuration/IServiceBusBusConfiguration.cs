namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IServiceBusBusConfiguration :
        IBusConfiguration,
        IServiceBusEndpointConfiguration
    {
        new IServiceBusHostConfiguration HostConfiguration { get; }

        new IServiceBusEndpointConfiguration BusEndpointConfiguration { get; }

        new IServiceBusTopologyConfiguration Topology { get; }

        IServiceBusEndpointConfiguration CreateEndpointConfiguration();
    }
}
