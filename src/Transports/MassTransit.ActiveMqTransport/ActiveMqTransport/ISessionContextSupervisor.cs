namespace MassTransit.ActiveMqTransport
{
    using Transports;


    /// <summary>
    /// Creates and caches a session on the connection
    /// </summary>
    public interface ISessionContextSupervisor :
        ITransportSupervisor<SessionContext>
    {
    }
}
