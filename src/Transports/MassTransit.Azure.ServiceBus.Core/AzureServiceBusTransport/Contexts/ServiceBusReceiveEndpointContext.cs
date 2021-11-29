namespace MassTransit.AzureServiceBusTransport
{
    using Topology;
    using Transports;


    public interface ServiceBusReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
