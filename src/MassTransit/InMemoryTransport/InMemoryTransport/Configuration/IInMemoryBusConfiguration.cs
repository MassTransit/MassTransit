namespace MassTransit.InMemoryTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IInMemoryBusConfiguration :
        IBusConfiguration,
        IInMemoryEndpointConfiguration
    {
        new IInMemoryHostConfiguration HostConfiguration { get; }

        new IInMemoryEndpointConfiguration BusEndpointConfiguration { get; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IInMemoryEndpointConfiguration CreateEndpointConfiguration();
    }
}
