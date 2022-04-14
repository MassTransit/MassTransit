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
    }
}
