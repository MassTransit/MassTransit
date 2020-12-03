namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using GreenPipes.Agents;
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IConnectionContextSupervisor supervisor, IPipeContextFactory<ClientContext> contextFactory)
            : base(supervisor, contextFactory)
        {
        }
    }
}
