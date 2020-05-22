namespace MassTransit.ActiveMqTransport.Transport
{
    using GreenPipes.Agents;


    /// <summary>
    /// Attaches a connection context to the value (shared, of course)
    /// </summary>
    public interface IConnectionContextSupervisor :
        ISupervisor<ConnectionContext>,
        ISendTransportProvider,
        IPublishTransportProvider
    {
    }
}
