namespace MassTransit.InMemoryTransport
{
    using System;
    using Configuration;
    using Transports;


    /// <summary>
    /// Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHost :
        BaseHost,
        IInMemoryHost
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryHost(IInMemoryHostConfiguration hostConfiguration, IInMemoryBusTopology busTopology)
            : base(hostConfiguration, busTopology)
        {
            _hostConfiguration = hostConfiguration;
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

            configuration.Validate().ThrowIfContainsFailure("The receive endpoint configuration is invalid:");

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Add("type", "InMemory");
            context.Add("baseAddress", _hostConfiguration.HostAddress);

            _hostConfiguration.TransportProvider.Probe(context);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] { _hostConfiguration.TransportProvider };
        }
    }
}
