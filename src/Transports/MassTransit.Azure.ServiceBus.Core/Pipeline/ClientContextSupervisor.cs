namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using GreenPipes;
    using GreenPipes.Agents;


    public class ClientContextSupervisor :
        PipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IPipeContextFactory<ClientContext> contextFactory)
            : base(contextFactory)
        {
        }

        public void Probe(ProbeContext context)
        {
            if (HasContext)
                context.Add("connected", true);
        }
    }
}
