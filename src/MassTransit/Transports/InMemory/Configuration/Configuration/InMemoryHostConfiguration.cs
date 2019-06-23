namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using MassTransit.Topology;


    public class InMemoryHostConfiguration :
        IInMemoryHostConfiguration
    {
        readonly IInMemoryBusConfiguration _busConfiguration;
        readonly InMemoryHost _host;

        public InMemoryHostConfiguration(IInMemoryBusConfiguration busConfiguration, Uri baseAddress, int transportConcurrencyLimit, IHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;

            Topology = hostTopology;
            HostAddress = baseAddress ?? new Uri("loopback://localhost/");

            _host = new InMemoryHost(this, transportConcurrencyLimit);
        }

        public Uri HostAddress { get; }
        IBusHostControl IHostConfiguration.Host => _host;
        public IHostTopology Topology { get; }

        public bool Matches(Uri address)
        {
            return address.ToString().StartsWith(HostAddress.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            return _host.GetSendTransport(address);
        }

        public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IInMemoryEndpointConfiguration endpointConfiguration)
        {
            return new InMemoryReceiveEndpointConfiguration(this, queueName, endpointConfiguration ?? _busConfiguration.CreateEndpointConfiguration());
        }

        public IInMemoryHostControl Host => _host;
    }
}
