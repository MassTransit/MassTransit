namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;


    public class SendEndpointContextSupervisor :
        PipeContextSupervisor<SendEndpointContext>,
        ISendEndpointContextSupervisor
    {
        public SendEndpointContextSupervisor(IPipeContextFactory<SendEndpointContext> contextFactory)
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
