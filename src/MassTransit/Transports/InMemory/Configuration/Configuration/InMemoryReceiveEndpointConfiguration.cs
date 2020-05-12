namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Builders;
    using MassTransit.Configuration;


    public class InMemoryReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IInMemoryReceiveEndpointConfiguration,
        IInMemoryReceiveEndpointConfigurator
    {
        readonly IInMemoryEndpointConfiguration _endpointConfiguration;
        readonly string _queueName;

        public InMemoryReceiveEndpointConfiguration(IInMemoryHostConfiguration hostConfiguration, string queueName,
            IInMemoryEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));

            HostAddress = hostConfiguration?.HostAddress ?? throw new ArgumentNullException(nameof(hostConfiguration.HostAddress));

            InputAddress = new InMemoryEndpointAddress(hostConfiguration.HostAddress, queueName);
        }

        IInMemoryReceiveEndpointConfigurator IInMemoryReceiveEndpointConfiguration.Configurator => this;

        public int ConcurrencyLimit { get; set; }

        IInMemoryTopologyConfiguration IInMemoryEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public void Build(IInMemoryHostControl host)
        {
            var builder = new InMemoryReceiveEndpointBuilder(host, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            var transport = host.GetReceiveTransport(_queueName, receiveEndpointContext);

            var receiveEndpoint = new ReceiveEndpoint(transport, receiveEndpointContext);

            host.AddReceiveEndpoint(_queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }
    }
}
