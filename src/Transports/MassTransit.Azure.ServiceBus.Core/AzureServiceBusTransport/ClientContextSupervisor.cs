namespace MassTransit.AzureServiceBusTransport
{
    using Agents;
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
