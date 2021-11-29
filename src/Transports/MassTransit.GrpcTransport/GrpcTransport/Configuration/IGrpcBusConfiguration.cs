namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IGrpcBusConfiguration :
        IBusConfiguration,
        IGrpcEndpointConfiguration
    {
        new IGrpcHostConfiguration HostConfiguration { get; }

        new IGrpcEndpointConfiguration BusEndpointConfiguration { get; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IGrpcEndpointConfiguration CreateEndpointConfiguration();
    }
}
