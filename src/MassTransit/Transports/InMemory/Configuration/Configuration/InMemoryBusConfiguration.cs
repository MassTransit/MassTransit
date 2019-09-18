namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using Pipeline.Observables;


    public class InMemoryBusConfiguration :
        InMemoryEndpointConfiguration,
        IInMemoryBusConfiguration
    {
        readonly IInMemoryEndpointConfiguration _busEndpointConfiguration;
        readonly BusObservable _busObservers;
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryBusConfiguration(IInMemoryTopologyConfiguration topologyConfiguration, Uri baseAddress)
            : base(topologyConfiguration)
        {
            _hostConfiguration = new InMemoryHostConfiguration(this, baseAddress, topologyConfiguration);
            _busEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => _hostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        IInMemoryEndpointConfiguration IInMemoryBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IInMemoryHostConfiguration IInMemoryBusConfiguration.HostConfiguration => _hostConfiguration;

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busObservers.Connect(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _hostConfiguration.ConnectEndpointConfigurationObserver(observer);
        }
    }
}
