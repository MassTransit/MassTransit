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
        readonly IHttpEndpointConfiguration _busEndpointConfiguration;
        readonly BusObservable _busObservers;
        readonly IHttpHostConfiguration _hostConfiguration;

        public HttpBusConfiguration(IHttpTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _hostConfiguration = new HttpHostConfiguration(this, topologyConfiguration);
            _busEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => _hostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        IHttpEndpointConfiguration IHttpBusConfiguration.BusEndpointConfiguration => _busEndpointConfiguration;
        IHttpHostConfiguration IHttpBusConfiguration.HostConfiguration => _hostConfiguration;

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
