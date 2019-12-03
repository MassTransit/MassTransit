namespace MassTransit.AmazonSqsTransport.Contexts
{
    using Context;
    using Topology.Builders;


    public interface SqsReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }
    }
}
