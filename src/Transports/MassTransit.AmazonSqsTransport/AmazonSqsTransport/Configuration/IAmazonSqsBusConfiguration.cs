namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IAmazonSqsBusConfiguration :
        IBusConfiguration
    {
        new IAmazonSqsHostConfiguration HostConfiguration { get; }

        new IAmazonSqsEndpointConfiguration BusEndpointConfiguration { get; }

        new IAmazonSqsTopologyConfiguration Topology { get; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IAmazonSqsEndpointConfiguration CreateEndpointConfiguration();
    }
}
