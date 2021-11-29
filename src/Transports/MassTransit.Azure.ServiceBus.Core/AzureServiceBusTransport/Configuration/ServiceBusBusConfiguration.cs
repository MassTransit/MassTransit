namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using MassTransit.Configuration;
    using Observables;


    public class ServiceBusBusConfiguration :
        ServiceBusEndpointConfiguration,
        IServiceBusBusConfiguration
    {
        readonly BusObservable _busObservers;

        public ServiceBusBusConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            HostConfiguration = new ServiceBusHostConfiguration(this, topologyConfiguration);
            BusEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => HostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => BusEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        public IServiceBusEndpointConfiguration BusEndpointConfiguration { get; }
        public IServiceBusHostConfiguration HostConfiguration { get; }

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
