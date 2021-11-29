namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IServiceBusEndpointConfiguration :
        IEndpointConfiguration
    {
        new IServiceBusTopologyConfiguration Topology { get; }
    }
}
