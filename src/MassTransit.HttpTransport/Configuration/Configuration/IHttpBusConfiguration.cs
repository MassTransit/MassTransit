namespace MassTransit.HttpTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IHttpBusConfiguration :
        IBusConfiguration
    {
        new IHttpHostConfiguration HostConfiguration { get; }

        new IHttpEndpointConfiguration BusEndpointConfiguration { get; }

        new IHttpTopologyConfiguration Topology { get; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IHttpEndpointConfiguration CreateEndpointConfiguration();
   }
}
