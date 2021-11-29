namespace MassTransit.AmazonSqsTransport.Configuration
{
    using MassTransit.Configuration;
    using Observables;


    public class AmazonSqsBusConfiguration :
        AmazonSqsEndpointConfiguration,
        IAmazonSqsBusConfiguration
    {
        readonly BusObservable _busObservers;

        public AmazonSqsBusConfiguration(IAmazonSqsTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            HostConfiguration = new AmazonSqsHostConfiguration(this, topologyConfiguration);
            BusEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => HostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => BusEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        public IAmazonSqsEndpointConfiguration BusEndpointConfiguration { get; }
        public IAmazonSqsHostConfiguration HostConfiguration { get; }

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
