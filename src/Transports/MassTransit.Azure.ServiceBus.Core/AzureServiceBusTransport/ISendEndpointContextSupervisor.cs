namespace MassTransit.AzureServiceBusTransport
{
    using Transports;


    public interface ISendEndpointContextSupervisor :
        ITransportSupervisor<SendEndpointContext>
    {
    }
}
