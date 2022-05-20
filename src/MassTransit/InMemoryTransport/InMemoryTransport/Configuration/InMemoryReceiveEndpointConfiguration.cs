#nullable enable
namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Transports;
    using Transports.Fabric;


    public class InMemoryReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IInMemoryReceiveEndpointConfiguration,
        IInMemoryReceiveEndpointConfigurator
    {
        readonly IInMemoryEndpointConfiguration _endpointConfiguration;
        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly string _queueName;

        public InMemoryReceiveEndpointConfiguration(IInMemoryHostConfiguration hostConfiguration, string queueName,
            IInMemoryEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));

            HostAddress = hostConfiguration?.HostAddress ?? throw new ArgumentNullException(nameof(hostConfiguration.HostAddress));

            InputAddress = new InMemoryEndpointAddress(hostConfiguration.HostAddress, queueName);
        }

        IInMemoryReceiveEndpointConfigurator IInMemoryReceiveEndpointConfiguration.Configurator => this;

        IInMemoryTopologyConfiguration IInMemoryEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateInMemoryReceiveEndpointContext();
        }

        public void Build(IHost host)
        {
            var context = CreateInMemoryReceiveEndpointContext();

            var transport = new InMemoryReceiveTransport(context, _queueName);

            var receiveEndpoint = new ReceiveEndpoint(transport, context);

            host.AddReceiveEndpoint(_queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public void Bind(string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut, string? routingKey = default)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            _endpointConfiguration.Topology.Consume.Bind(exchangeName, exchangeType, routingKey);
        }

        public void Bind<T>(ExchangeType exchangeType, string? routingKey = default)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Bind(exchangeType, routingKey);
        }

        InMemoryReceiveEndpointContext CreateInMemoryReceiveEndpointContext()
        {
            var builder = new InMemoryReceiveEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
        }
    }
}
