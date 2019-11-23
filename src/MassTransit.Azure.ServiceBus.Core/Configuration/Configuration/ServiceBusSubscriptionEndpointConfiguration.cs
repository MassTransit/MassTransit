namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
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
            SubscriptionEndpointSettings settings, IServiceBusEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, settings, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
            _settings = settings;

            HostAddress = hostConfiguration.HostAddress;
            InputAddress = settings.GetInputAddress(hostConfiguration.HostAddress, settings.Path);
        }

        IServiceBusSubscriptionEndpointConfigurator IServiceBusSubscriptionEndpointConfiguration.Configurator => this;

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

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

        public void Build(IServiceBusHostControl host)
        {
            var builder = new ServiceBusSubscriptionEndpointBuilder(host, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions, host.Stopping));

            CreateReceiveEndpoint(host, receiveEndpointContext);
        }

        protected override IErrorTransport CreateErrorTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.SubscriptionConfigurator, _hostConfiguration.HostAddress);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextSupervisor(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.SubscriptionConfigurator, _hostConfiguration.HostAddress);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextSupervisor(host, settings));
        }

        protected override IClientContextSupervisor CreateClientCache(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor)
        {
            return new ClientContextSupervisor(new SubscriptionClientContextFactory(messagingFactoryContextSupervisor, namespaceContextSupervisor,
                MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(), _settings));
        }
    }
}
