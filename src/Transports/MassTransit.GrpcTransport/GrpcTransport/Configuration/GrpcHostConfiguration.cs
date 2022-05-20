namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Grpc.Core;
    using MassTransit.Configuration;
    using Topology;
    using Transports;
    using Util;


    public class GrpcHostConfiguration :
        BaseHostConfiguration<IGrpcReceiveEndpointConfiguration, IGrpcReceiveEndpointConfigurator>,
        IGrpcHostConfiguration,
        IGrpcHostConfigurator
    {
        static readonly Uri _defaultHostAddress = new Uri("http://127.0.0.1:0/");

        readonly IGrpcBusConfiguration _busConfiguration;
        readonly IList<GrpcServerConfiguration> _serverConfigurations;
        readonly GrpcBusTopology _topology;
        readonly Recycle<IGrpcTransportProvider> _transportProvider;
        bool _anyReceiveEndpointConfigured;
        Uri _baseAddress;
        Uri _hostAddress;

        public GrpcHostConfiguration(IGrpcBusConfiguration busConfiguration, Uri baseAddress, IGrpcTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;

            BaseAddress = baseAddress;

            _topology = new GrpcBusTopology(this, topologyConfiguration);

            _serverConfigurations = new List<GrpcServerConfiguration>();

            ReceiveTransportRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ConnectionException>();
                x.Handle<RpcException>();

                x.Exponential(1000, TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            _transportProvider = new Recycle<IGrpcTransportProvider>(() => new GrpcTransportProvider(this, topologyConfiguration));
        }

        public override Uri HostAddress => _hostAddress ??= _transportProvider.Supervisor.HostAddress;
        public override IBusTopology Topology => _topology;

        public IEnumerable<GrpcServerConfiguration> ServerConfigurations => _serverConfigurations;

        public override IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public Uri BaseAddress
        {
            get => _baseAddress ?? _defaultHostAddress;
            set => _baseAddress = value ?? _defaultHostAddress;
        }

        IGrpcHostConfigurator IGrpcHostConfiguration.Configurator => this;
        IGrpcTransportProvider IGrpcHostConfiguration.TransportProvider => _transportProvider.Supervisor;
        IGrpcBusTopology IGrpcHostConfiguration.Topology => _topology;

        public void ApplyEndpointDefinition(IGrpcReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            base.ApplyEndpointDefinition(configurator, definition);
        }

        public IGrpcReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IGrpcReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            return CreateReceiveEndpointConfiguration(queueName, endpointConfiguration, configure);
        }

        public IGrpcReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            IGrpcEndpointConfiguration endpointConfiguration, Action<IGrpcReceiveEndpointConfigurator> configure)
        {
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(queueName));

            var configuration = new GrpcReceiveEndpointConfiguration(this, queueName, endpointConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);
            Add(configuration);

            _anyReceiveEndpointConfigured = true;

            return configuration;
        }

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IGrpcReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override void ReceiveEndpoint(string queueName, Action<IGrpcReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IReceiveEndpointConfigurator> configure = null)
        {
            return CreateReceiveEndpointConfiguration(queueName, configure);
        }

        public override IHost Build()
        {
            var host = new GrpcHost(this, _topology);

            foreach (var endpointConfiguration in GetConfiguredEndpoints())
                endpointConfiguration.Build(host);

            return host;
        }

        public string Host
        {
            set
            {
                if (_anyReceiveEndpointConfigured)
                    throw new ConfigurationException("The host must be configured before any receive endpoints");

                _baseAddress = new UriBuilder(_baseAddress) { Host = value }.Uri;
            }
        }

        public int Port
        {
            set
            {
                if (_anyReceiveEndpointConfigured)
                    throw new ConfigurationException("The host must be configured before any receive endpoints");

                _baseAddress = new UriBuilder(_baseAddress) { Port = value }.Uri;
            }
        }

        public void AddServer(Uri address)
        {
            _serverConfigurations.Add(new GrpcServerConfiguration(address));
        }
    }
}
