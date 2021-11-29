namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;
    using Transports;


    public interface IServiceBusEntityEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IServiceBusEndpointConfiguration
    {
        void Build(IHost host);
    }
}
