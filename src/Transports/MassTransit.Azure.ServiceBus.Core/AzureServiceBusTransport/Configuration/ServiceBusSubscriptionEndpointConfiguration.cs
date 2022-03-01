namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Configuration;
    using Middleware;
    using Topology;
    using Transports;


    public class ServiceBusSubscriptionEndpointConfiguration :
        ServiceBusEntityReceiveEndpointConfiguration,
        IServiceBusSubscriptionEndpointConfiguration,
        IServiceBusSubscriptionEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly SubscriptionEndpointSettings _settings;

        public ServiceBusSubscriptionEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration,
            SubscriptionEndpointSettings settings, IServiceBusEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, settings, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
            _settings = settings;

            HostAddress = hostConfiguration.HostAddress;
            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public SubscriptionSettings Settings => _settings;

        public override Uri HostAddress { get; }

        public override Uri InputAddress => _inputAddress.Value;

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateServiceBusReceiveEndpointContext();
        }

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.SubscriptionConfigurator.Validate()
                .Concat(base.Validate());
        }

        public void Build(IHost host)
        {
            this.ConfigureDeadLetterQueueDeadLetterTransport();
            this.ConfigureDeadLetterQueueErrorTransport();

            var context = CreateServiceBusReceiveEndpointContext();

            ClientPipeConfigurator.UseFilter(new ConfigureServiceBusTopologyFilter<SubscriptionSettings>(_settings, context.BrokerTopology,
                _settings.RemoveSubscriptions, context));

            CreateReceiveEndpoint(host, context);
        }

        public RuleFilter Filter
        {
            set => _settings.Filter = value;
        }

        public CreateRuleOptions Rule
        {
            set => _settings.Rule = value;
        }

        ServiceBusReceiveEndpointContext CreateServiceBusReceiveEndpointContext()
        {
            var builder = new ServiceBusSubscriptionEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
        }

        Uri FormatInputAddress()
        {
            return _settings.GetInputAddress(_hostConfiguration.HostAddress, _settings.Path);
        }
    }
}
