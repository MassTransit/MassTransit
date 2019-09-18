namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Observables;


    public class ServiceBusBusConfiguration :
        ServiceBusEndpointConfiguration,
        IServiceBusBusConfiguration
    {
        readonly IServiceBusEndpointConfiguration _busEndpointConfiguration;
        readonly BusObservable _busObservers;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusBusConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _hostConfiguration = new ServiceBusHostConfiguration(this, topologyConfiguration);
            _busEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => _hostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        IServiceBusEndpointConfiguration IServiceBusBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IServiceBusHostConfiguration IServiceBusBusConfiguration.HostConfiguration => _hostConfiguration;

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
