namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IAmazonSqsEndpointConfiguration :
        IEndpointConfiguration
    {
        new IAmazonSqsTopologyConfiguration Topology { get; }
    }
}
