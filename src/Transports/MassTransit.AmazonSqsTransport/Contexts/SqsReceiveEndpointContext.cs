namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Context;
    using Topology.Builders;
    using Transport;


    public interface SqsReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
