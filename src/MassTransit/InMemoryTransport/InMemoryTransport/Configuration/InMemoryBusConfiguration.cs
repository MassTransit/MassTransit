namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Observables;


    public class InMemoryBusConfiguration :
        InMemoryEndpointConfiguration,
        IInMemoryBusConfiguration
    {
        readonly BusObservable _busObservers;

        public InMemoryBusConfiguration(IInMemoryTopologyConfiguration topologyConfiguration, Uri baseAddress)
            : base(topologyConfiguration)
        {
            HostConfiguration = new InMemoryHostConfiguration(this, baseAddress, topologyConfiguration);
            BusEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => HostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => BusEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        public IInMemoryEndpointConfiguration BusEndpointConfiguration { get; }
        public IInMemoryHostConfiguration HostConfiguration { get; }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busObservers.Connect(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return HostConfiguration.ConnectEndpointConfigurationObserver(observer);
        }
    }
}
