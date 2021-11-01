namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Contexts;
    using GreenPipes.Agents;
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IPipeContextFactory<ClientContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}
