namespace MassTransit.AmazonSqsTransport
{
    using MassTransit.Topology;
    using Topology;
    using Transports;


    public interface IAmazonSqsHost :
        IHost<IAmazonSqsReceiveEndpointConfigurator>
    {
        new IAmazonSqsBusTopology Topology { get; }
    }
}
