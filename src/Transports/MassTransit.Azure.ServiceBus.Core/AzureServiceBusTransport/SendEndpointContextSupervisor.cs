namespace MassTransit.AzureServiceBusTransport
{
    using Agents;
    using Transports;


    public class SendEndpointContextSupervisor :
        TransportPipeContextSupervisor<SendEndpointContext>,
        ISendEndpointContextSupervisor
    {
        public SendEndpointContextSupervisor(IPipeContextFactory<SendEndpointContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}
