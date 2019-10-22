namespace MassTransit.Configuration
{
    using BusConfigurators;
    using EndpointConfigurators;


    /// <summary>
    /// The configuration of a bus
    /// </summary>
    public interface IBusConfiguration :
        IEndpointConfiguration,
        IBusObserverConnector,
        IEndpointConfigurationObserverConnector
    {
        IHostConfiguration HostConfiguration { get; }

        IEndpointConfiguration BusEndpointConfiguration { get; }

        IBusObserver BusObservers { get; }
    }
}
