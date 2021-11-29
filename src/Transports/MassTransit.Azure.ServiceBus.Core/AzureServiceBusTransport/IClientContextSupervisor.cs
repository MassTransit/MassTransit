namespace MassTransit.AzureServiceBusTransport
{
    using Transports;


    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
    }
}
