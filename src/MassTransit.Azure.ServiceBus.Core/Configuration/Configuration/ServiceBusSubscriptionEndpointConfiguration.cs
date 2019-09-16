namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Microsoft.Azure.ServiceBus;
    using Pipeline;
    using Settings;
    using Transport;
    using Transports;


    public class ServiceBusSubscriptionEndpointConfiguration :
        ServiceBusEntityReceiveEndpointConfiguration,
        IServiceBusSubscriptionEndpointConfiguration,
        IServiceBusSubscriptionEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly SubscriptionEndpointSettings _settings;

        public ServiceBusSubscriptionEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration,
            IServiceBusEndpointConfiguration endpointConfiguration,
            string subscriptionName, string topicPath)
            : this(hostConfiguration, endpointConfiguration, new SubscriptionEndpointSettings(topicPath, subscriptionName))
        {
        }

        public ServiceBusSubscriptionEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration,
            IServiceBusEndpointConfiguration endpointConfiguration,
            SubscriptionEndpointSettings settings)
            : base(hostConfiguration, endpointConfiguration, settings)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
            _settings = settings;

            HostAddress = hostConfiguration.HostAddress;
            InputAddress = settings.GetInputAddress(hostConfiguration.HostAddress, settings.Path);
        }

        IServiceBusSubscriptionEndpointConfigurator IServiceBusSubscriptionEndpointConfiguration.Configurator => this;

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public IServiceBusEndpointConfiguration CreateEndpointConfiguration()
        {
            return _endpointConfiguration.CreateEndpointConfiguration();
        }

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public IServiceBusHost Host => _hostConfiguration.Host;

        public Filter Filter
        {
            set => _settings.Filter = value;
        }

        public RuleDescription Rule
        {
            set => _settings.Rule = value;
        }

        public SubscriptionSettings Settings => _settings;

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.SubscriptionConfigurator.Validate()
                .Concat(base.Validate());
        }

        public override IReceiveEndpoint Build()
        {
            var builder = new ServiceBusSubscriptionEndpointBuilder(this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions, _hostConfiguration.Host.Stopping));

            return CreateReceiveEndpoint(receiveEndpointContext);
        }

        protected override IErrorTransport CreateErrorTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.SubscriptionConfigurator,
                _hostConfiguration.Host.Address.AbsolutePath);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextSupervisor(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.SubscriptionConfigurator,
                _hostConfiguration.Host.Address.AbsolutePath);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextSupervisor(host, settings));
        }

        protected override IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryContextSupervisor, host.NamespaceContextSupervisor, Pipe.Empty<MessagingFactoryContext>(), namespacePipe,
                settings);
        }

        protected override IClientContextSupervisor CreateClientCache(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor, INamespaceContextSupervisor namespaceContextSupervisor)
        {
            return new ClientContextSupervisor(new SubscriptionClientContextFactory(messagingFactoryContextSupervisor, namespaceContextSupervisor,
                MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(), _settings));
        }
    }
}
