namespace MassTransit.EventHubIntegration.Contexts
{
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
    }
}
