namespace MassTransit.AzureServiceBusTransport.Configuration
{
    public interface IServiceBusReceiveEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        ReceiveSettings Settings { get; }
    }
}
