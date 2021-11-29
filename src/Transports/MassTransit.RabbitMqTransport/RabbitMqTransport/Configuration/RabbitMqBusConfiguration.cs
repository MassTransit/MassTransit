namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Observables;


    public class RabbitMqBusConfiguration :
        RabbitMqEndpointConfiguration,
        IRabbitMqBusConfiguration
    {
        readonly BusObservable _busObservers;

        public RabbitMqBusConfiguration(IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            HostConfiguration = new RabbitMqHostConfiguration(this, topologyConfiguration);
            BusEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => HostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => BusEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        public IRabbitMqEndpointConfiguration BusEndpointConfiguration { get; }
        public IRabbitMqHostConfiguration HostConfiguration { get; }

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
