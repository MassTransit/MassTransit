namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using Configuration;
    using Context;
    using Definition;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Configurators;
    using Settings;
    using Topology;
    using Transports;


    public class ServiceBusHost :
        BaseHost,
        IServiceBusHost
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusHost(IServiceBusHostConfiguration hostConfiguration, IServiceBusHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            Topology = hostTopology;

            Add(hostConfiguration.ConnectionContextSupervisor);
        }

        public IServiceBusHostTopology Topology { get; }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "Azure Service Bus",
                _hostConfiguration.HostAddress,
                _hostConfiguration.Settings.OperationTimeout
            });

            _hostConfiguration.ConnectionContextSupervisor.Probe(context);
        }

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
            Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

            configuration.Build(this);

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            var settings = new SubscriptionEndpointSettings(Topology.Publish<T>().TopicDescription, subscriptionName);

            return ConnectSubscriptionEndpoint(settings, configure);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            var settings = new SubscriptionEndpointSettings(topicName, subscriptionName);

            return ConnectSubscriptionEndpoint(settings, configure);
        }

        HostReceiveEndpointHandle ConnectSubscriptionEndpoint(SubscriptionEndpointSettings settings,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            LogContext.Debug?.Log("Connect subscription endpoint: {Topic}/{SubscriptionName}", settings.Path, settings.Name);

            var configuration = _hostConfiguration.CreateSubscriptionEndpointConfiguration(settings, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {_hostConfiguration.ConnectionContextSupervisor};
        }
    }
}
