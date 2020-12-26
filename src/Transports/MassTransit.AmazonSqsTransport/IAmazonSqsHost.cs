namespace MassTransit.AmazonSqsTransport
{
    using Topology;


    public interface IAmazonSqsHost :
        IHost<IAmazonSqsReceiveEndpointConfigurator>
    {
        new IAmazonSqsHostTopology Topology { get; }
    }
}
