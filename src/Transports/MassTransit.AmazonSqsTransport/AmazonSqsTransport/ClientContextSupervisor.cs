namespace MassTransit.AmazonSqsTransport
{
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IConnectionContextSupervisor connectionContextSupervisor)
            : base(new ClientContextFactory(connectionContextSupervisor))
        {
            connectionContextSupervisor.AddConsumeAgent(this);
        }

        public ClientContextSupervisor(IClientContextSupervisor clientContextSupervisor)
            : base(new ScopeClientContextFactory(clientContextSupervisor))
        {
            clientContextSupervisor.AddSendAgent(this);
        }
    }
}
