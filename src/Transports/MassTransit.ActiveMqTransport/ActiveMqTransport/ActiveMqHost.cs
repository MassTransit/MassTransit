namespace MassTransit.ActiveMqTransport
{
    using System;
    using Configuration;
    using Transports;


    public class ActiveMqHost :
        BaseHost,
        IActiveMqHost
    {
        readonly IActiveMqHostConfiguration _hostConfiguration;

        public ActiveMqHost(IActiveMqHostConfiguration hostConfiguration, IActiveMqBusTopology busTopology)
            : base(hostConfiguration, busTopology)
        {
            _hostConfiguration = hostConfiguration;
            Topology = busTopology;
        }

        public new IActiveMqBusTopology Topology { get; }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            configuration.Validate().ThrowIfContainsFailure("The receive endpoint configuration is invalid:");

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "ActiveMQ",
                _hostConfiguration.Settings.Host,
                _hostConfiguration.Settings.Port,
                _hostConfiguration.Settings.Username,
                Password = new string('*', _hostConfiguration.Settings.Password.Length)
            });

            _hostConfiguration.ConnectionContextSupervisor.Probe(context);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] { _hostConfiguration.ConnectionContextSupervisor };
        }
    }
}
