namespace MassTransit.AzureServiceBusTransport
{
    using Agents;


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
