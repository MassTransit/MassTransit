namespace MassTransit.ActiveMqTransport.Transport
{
    using Transports;


    public class SessionContextSupervisor :
        TransportPipeContextSupervisor<SessionContext>,
        ISessionContextSupervisor

    {
        public SessionContextSupervisor(IConnectionContextSupervisor supervisor)
            : base(supervisor, new SessionContextFactory(supervisor))
        {
        }
    }
}
