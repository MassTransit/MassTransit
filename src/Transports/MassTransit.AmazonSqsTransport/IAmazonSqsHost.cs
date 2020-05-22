namespace MassTransit.AmazonSqsTransport
{
    using Topology;


    public interface IAmazonSqsHost :
        IHost,
        IReceiveConnector<IAmazonSqsReceiveEndpointConfigurator>
    {
        new IAmazonSqsHostTopology Topology { get; }
    }
}
