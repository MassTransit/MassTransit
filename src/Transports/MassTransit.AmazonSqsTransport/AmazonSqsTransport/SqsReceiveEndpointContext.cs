namespace MassTransit.AmazonSqsTransport
{
    using Topology;
    using Transports;


    public interface SqsReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
