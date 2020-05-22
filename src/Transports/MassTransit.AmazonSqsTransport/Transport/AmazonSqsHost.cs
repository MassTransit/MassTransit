namespace MassTransit.AmazonSqsTransport.Transport
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


    public class AmazonSqsHost :
        BaseHost,
        IAmazonSqsHost
    {
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public AmazonSqsHost(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            Topology = hostTopology;

            Add(_hostConfiguration.ConnectionContextSupervisor);
        }

        public IAmazonSqsHostTopology Topology { get; }

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
            Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configure = null)
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
                Type = "AmazonSQS",
                _hostConfiguration.Settings.Region,
                _hostConfiguration.Settings.AccessKey
            });

            _hostConfiguration.ConnectionContextSupervisor.Probe(context);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {_hostConfiguration.ConnectionContextSupervisor};
        }
    }
}
