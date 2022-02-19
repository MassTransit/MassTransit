namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Transports;
    using Transports.Fabric;


    public class GrpcReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IGrpcReceiveEndpointConfiguration,
        IGrpcReceiveEndpointConfigurator
    {
        readonly IGrpcEndpointConfiguration _endpointConfiguration;
        readonly IGrpcHostConfiguration _hostConfiguration;
        readonly string _queueName;

        public GrpcReceiveEndpointConfiguration(IGrpcHostConfiguration hostConfiguration, string queueName,
            IGrpcEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));

            HostAddress = hostConfiguration?.HostAddress ?? throw new ArgumentNullException(nameof(hostConfiguration.HostAddress));

            InputAddress = new GrpcEndpointAddress(hostConfiguration.HostAddress, queueName);
        }

        IGrpcReceiveEndpointConfigurator IGrpcReceiveEndpointConfiguration.Configurator => this;

        IGrpcTopologyConfiguration IGrpcEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateGrpcReceiveEndpointContext();
        }

        public void Build(IHost host)
        {
            var context = CreateGrpcReceiveEndpointContext();

            var transport = new GrpcReceiveTransport(context, _queueName);

            var receiveEndpoint = new ReceiveEndpoint(transport, context);

            host.AddReceiveEndpoint(_queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public void Bind(string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut, string routingKey = default)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            _endpointConfiguration.Topology.Consume.Bind(exchangeName, exchangeType, routingKey);
        }

        public void Bind<T>(ExchangeType? exchangeType, string routingKey = default)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Bind(exchangeType, routingKey);
        }

        GrpcReceiveEndpointContext CreateGrpcReceiveEndpointContext()
        {
            var builder = new GrpcReceiveEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
        }
    }
}
