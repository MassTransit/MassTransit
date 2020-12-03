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

        public SubscriptionSettings Settings => _settings;

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.SubscriptionConfigurator.Validate()
                .Concat(base.Validate());
        }

        public void Build(IHost host)
        {
            var builder = new ServiceBusSubscriptionEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            ClientPipeConfigurator.UseFilter(new ConfigureTopologyFilter<SubscriptionSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions, _hostConfiguration.ConnectionContextSupervisor.Stopping));

            CreateReceiveEndpoint(host, receiveEndpointContext);
        }

        public Filter Filter
        {
            set => _settings.Filter = value;
        }

        public RuleDescription Rule
        {
            set => _settings.Rule = value;
        }

        protected override IErrorTransport CreateErrorTransport()
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.SubscriptionConfigurator, _hostConfiguration.HostAddress);

            return new BrokeredMessageErrorTransport(_hostConfiguration.CreateSendEndpointContextSupervisor(settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport()
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.SubscriptionConfigurator, _hostConfiguration.HostAddress);

            return new BrokeredMessageDeadLetterTransport(_hostConfiguration.CreateSendEndpointContextSupervisor(settings));
        }
    }
}
