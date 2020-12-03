namespace MassTransit.AmazonSqsTransport.Transport
{
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IConnectionContextSupervisor supervisor)
            : base(supervisor, new ClientContextFactory(supervisor))
        {
        }
    }
}
