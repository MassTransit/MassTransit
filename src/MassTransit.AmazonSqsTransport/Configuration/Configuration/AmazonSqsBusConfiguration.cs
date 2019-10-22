namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Observables;


    public class AmazonSqsBusConfiguration :
        AmazonSqsEndpointConfiguration,
        IAmazonSqsBusConfiguration
    {
        readonly IAmazonSqsEndpointConfiguration _busEndpointConfiguration;
        readonly BusObservable _busObservers;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public AmazonSqsBusConfiguration(IAmazonSqsTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _hostConfiguration = new AmazonSqsHostConfiguration(this, topologyConfiguration);
            _busEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => _hostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        IAmazonSqsEndpointConfiguration IAmazonSqsBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IAmazonSqsHostConfiguration IAmazonSqsBusConfiguration.HostConfiguration => _hostConfiguration;

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
