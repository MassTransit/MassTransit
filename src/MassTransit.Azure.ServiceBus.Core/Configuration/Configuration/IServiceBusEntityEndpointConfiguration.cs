namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Configuration;


    public interface IServiceBusEntityEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IServiceBusEndpointConfiguration
    {
        void Build(IServiceBusHostControl host);
    }
}
