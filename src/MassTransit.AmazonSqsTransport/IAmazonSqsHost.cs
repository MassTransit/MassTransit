namespace MassTransit.AmazonSqsTransport
{
    using Configuration;
    using GreenPipes;
    using Topology;
    using Transport;


    public interface IAmazonSqsHost :
        IHost,
        IReceiveConnector<IAmazonSqsReceiveEndpointConfigurator>
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IRetryPolicy ConnectionRetryPolicy { get; }

        AmazonSqsHostSettings Settings { get; }

        new IAmazonSqsHostTopology Topology { get; }
    }
}
