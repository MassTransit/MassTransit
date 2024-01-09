namespace MassTransit.SqlTransport
{
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(new ScopeClientContextFactory(connectionContextSupervisor))
        {
            connectionContextSupervisor.AddConsumeAgent(this);
        }

        public ClientContextSupervisor(IClientContextSupervisor clientContextSupervisor)
            : base(new SharedClientContextFactory(clientContextSupervisor))
        {
            clientContextSupervisor.AddSendAgent(this);
        }
    }
}
