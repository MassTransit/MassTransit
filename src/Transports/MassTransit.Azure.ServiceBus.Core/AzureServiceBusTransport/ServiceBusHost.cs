namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Configuration;
    using Transports;


    public class ServiceBusHost :
        BaseHost,
        IServiceBusHost
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusHost(IServiceBusHostConfiguration hostConfiguration, IServiceBusBusTopology busTopology)
            : base(hostConfiguration, busTopology)
        {
            _hostConfiguration = hostConfiguration;
            Topology = busTopology;
        }

        public new IServiceBusBusTopology Topology { get; }

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

            configuration.Validate().ThrowIfContainsFailure("The receive endpoint configuration is invalid:");

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

            configuration.Build(this);

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointConfiguration = _hostConfiguration.CreateSubscriptionEndpointConfiguration<T>(subscriptionName, configure);

            return ConnectSubscriptionEndpoint(endpointConfiguration);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointConfiguration = _hostConfiguration.CreateSubscriptionEndpointConfiguration(subscriptionName, topicName, configure);

            return ConnectSubscriptionEndpoint(endpointConfiguration);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "Azure Service Bus",
                _hostConfiguration.HostAddress,
            });

            _hostConfiguration.ConnectionContextSupervisor.Probe(context);
        }

        HostReceiveEndpointHandle ConnectSubscriptionEndpoint(IServiceBusSubscriptionEndpointConfiguration configuration)
        {
            LogContext.Debug?.Log("Connect subscription endpoint: {Topic}/{SubscriptionName}", configuration.Settings.Path, configuration.Settings.Name);

            configuration.Validate().ThrowIfContainsFailure("The subscription endpoint configuration is invalid:");

            configuration.Build(this);

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] { _hostConfiguration.ConnectionContextSupervisor };
        }
    }
}
