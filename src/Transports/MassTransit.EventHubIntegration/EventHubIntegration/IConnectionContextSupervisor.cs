namespace MassTransit.EventHubIntegration
{
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
    }
}
