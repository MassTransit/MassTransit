namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Clients;
    using Context;
    using Hosting;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Transport;
    using Transports;


    public class HttpHostConfiguration :
        IHttpHostConfiguration
    {
        readonly IHttpBusConfiguration _busConfiguration;
        readonly HttpHostSettings _settings;
        readonly IHostTopology _hostTopology;
        readonly HttpHost _host;

        public HttpHostConfiguration(IHttpBusConfiguration busConfiguration, HttpHostSettings settings, IHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;
            _settings = settings;
            _hostTopology = hostTopology;

            HostAddress = settings.GetInputAddress();

            _host = new HttpHost(this);
        }

        public Uri HostAddress { get; }

        IBusHostControl IHostConfiguration.Host => _host;
        IHostTopology IHostConfiguration.Topology => _hostTopology;

        IHttpHost IHttpHostConfiguration.Host => _host;
        IHttpBusConfiguration IHttpHostConfiguration.BusConfiguration => _busConfiguration;
        HttpHostSettings IHttpHostConfiguration.Settings => _settings;

        public bool Matches(Uri address)
        {
            var settings = address.GetHostSettings();

            return HttpHostEqualityComparer.Default.Equals(_settings, settings);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            throw new NotImplementedException();
        }

        public IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string pathMatch)
        {
            return new HttpReceiveEndpointConfiguration(this, pathMatch, _busConfiguration.CreateEndpointConfiguration());
        }

        public Task<ISendTransport> CreateSendTransport(Uri address, ReceiveEndpointContext receiveEndpointContext)
        {
            var clientContextSupervisor = new HttpClientContextSupervisor(receiveEndpointContext.ReceivePipe);

            var sendSettings = address.GetSendSettings();

            var transport = new HttpSendTransport(clientContextSupervisor, sendSettings, receiveEndpointContext);

            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }
    }
}
