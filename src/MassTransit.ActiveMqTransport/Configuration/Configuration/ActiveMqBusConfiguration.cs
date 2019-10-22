namespace MassTransit.ActiveMqTransport.Configuration
{
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Observables;


    public class ActiveMqBusConfiguration :
        ActiveMqEndpointConfiguration,
        IActiveMqBusConfiguration
    {
        readonly IActiveMqEndpointConfiguration _busEndpointConfiguration;
        readonly BusObservable _busObservers;
        readonly IActiveMqHostConfiguration _hostConfiguration;

        public ActiveMqBusConfiguration(IActiveMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _hostConfiguration = new ActiveMqHostConfiguration(this, topologyConfiguration);
            _busEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => _hostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        IActiveMqEndpointConfiguration IActiveMqBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IActiveMqHostConfiguration IActiveMqBusConfiguration.HostConfiguration => _hostConfiguration;

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
