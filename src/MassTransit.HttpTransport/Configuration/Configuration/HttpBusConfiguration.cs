namespace MassTransit.HttpTransport.Configuration
{
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Observables;


    public class HttpBusConfiguration :
        HttpEndpointConfiguration,
        IHttpBusConfiguration
    {
        readonly BusObservable _busObservers;

        public HttpBusConfiguration(IHttpTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            HostConfiguration = new HttpHostConfiguration(this, topologyConfiguration);
            BusEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => HostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => BusEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        public IHttpEndpointConfiguration BusEndpointConfiguration { get; }
        public IHttpHostConfiguration HostConfiguration { get; }

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
