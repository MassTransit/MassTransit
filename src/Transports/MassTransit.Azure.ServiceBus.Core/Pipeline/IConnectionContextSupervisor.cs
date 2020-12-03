namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Contexts;
    using Transport;
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>,
        ISendTransportProvider,
        IPublishTransportProvider
    {
        ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(SendSettings settings);
    }
}
