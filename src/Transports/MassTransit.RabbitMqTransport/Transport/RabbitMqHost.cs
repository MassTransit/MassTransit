namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using Configuration;
    using Context;
    using Definition;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Configurators;
    using Topology;
    using Transports;


    public class RabbitMqHost :
        BaseHost,
        IRabbitMqHost
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public RabbitMqHost(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            Topology = hostTopology;
        }

        public new IRabbitMqHostTopology Topology { get; }

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
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "RabbitMQ",
                _hostConfiguration.Settings.Host,
                _hostConfiguration.Settings.Port,
                _hostConfiguration.Settings.VirtualHost,
                _hostConfiguration.Settings.Username,
                Password = new string('*', _hostConfiguration.Settings.Password.Length),
                _hostConfiguration.Settings.Heartbeat,
                _hostConfiguration.Settings.Ssl
            });

            if (_hostConfiguration.Settings.Ssl)
                context.Set(new {_hostConfiguration.Settings.SslServerName});

            _hostConfiguration.ConnectionContextSupervisor.Probe(context);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {_hostConfiguration.ConnectionContextSupervisor};
        }
    }
}
