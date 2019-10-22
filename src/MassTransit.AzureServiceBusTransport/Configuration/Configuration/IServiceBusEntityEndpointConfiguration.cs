namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IServiceBusEntityEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IServiceBusEndpointConfiguration
    {
        void Build(IServiceBusHostControl host);
    }
}
