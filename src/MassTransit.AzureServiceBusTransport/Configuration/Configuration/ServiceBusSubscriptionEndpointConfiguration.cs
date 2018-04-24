namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
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

            HostAddress = hostConfiguration.Host.Address;
            InputAddress = new UriBuilder(hostConfiguration.Host.Address)
            {
                Path = settings.Name
            }.Uri;
        }

        public override IReceiveEndpoint CreateReceiveEndpoint(string endpointName, IReceiveTransport receiveTransport, IReceivePipe receivePipe,
            ReceiveEndpointContext topology)
        {
            var receiveEndpoint = new ReceiveEndpoint(receiveTransport, receivePipe, topology);

            _hostConfiguration.Host.AddReceiveEndpoint(endpointName, receiveEndpoint);

            return receiveEndpoint;
        }

        public IServiceBusBusConfiguration BusConfiguration => _hostConfiguration.BusConfiguration;

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

            var receivePipe = CreateReceivePipe();

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions));

            return CreateReceiveEndpoint(builder, receivePipe, receiveEndpointContext);
        }

        protected override IErrorTransport CreateErrorTransport(ServiceBusHost host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.SubscriptionConfigurator,
                _hostConfiguration.Host.Address.AbsolutePath);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(ServiceBusHost host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.SubscriptionConfigurator,
                _hostConfiguration.Host.Address.AbsolutePath);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryCache, host.NamespaceCache, Pipe.Empty<MessagingFactoryContext>(), namespacePipe,
                settings);
        }

        protected override IClientCache CreateClientCache(Uri inputAddress, IMessagingFactoryCache messagingFactoryCache, INamespaceCache namespaceCache)
        {
            return new ClientCache(inputAddress, new SubscriptionClientContextFactory(messagingFactoryCache, namespaceCache,
                MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(), _settings));
        }
    }
}