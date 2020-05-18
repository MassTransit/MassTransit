namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using Transport;


    public interface IServiceBusReceiveEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        ReceiveSettings Settings { get; }
    }
}
