namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Configuration;


    public class SubscriptionEndpointSettings :
        BaseClientSettings,
        SubscriptionSettings
    {
        readonly CreateTopicOptions _createTopicOptions;
        readonly ServiceBusSubscriptionConfigurator _subscriptionConfigurator;

        public SubscriptionEndpointSettings(IServiceBusEndpointConfiguration configuration, string subscriptionName, string topicName)
            : this(configuration, subscriptionName, Defaults.GetCreateTopicOptions(topicName))
        {
        }

        public SubscriptionEndpointSettings(IServiceBusEndpointConfiguration configuration, string subscriptionName, CreateTopicOptions createTopicOptions)
            : this(configuration, createTopicOptions, new ServiceBusSubscriptionConfigurator(subscriptionName, createTopicOptions.Name))
        {
        }

        SubscriptionEndpointSettings(IServiceBusEndpointConfiguration configuration, CreateTopicOptions createTopicOptions,
            ServiceBusSubscriptionConfigurator configurator)
            : base(configuration, configurator)
        {
            _createTopicOptions = createTopicOptions;
            _subscriptionConfigurator = configurator;

            Name = Path = EntityNameFormatter.FormatSubscriptionPath(_subscriptionConfigurator.TopicPath, _subscriptionConfigurator.SubscriptionName);
        }

        public IServiceBusSubscriptionConfigurator SubscriptionConfigurator => _subscriptionConfigurator;

        public override bool RequiresSession => _subscriptionConfigurator.RequiresSession ?? false;
        public override int MaxConcurrentCallsPerSession => _subscriptionConfigurator.MaxConcurrentCallsPerSession ?? 1;

        CreateTopicOptions SubscriptionSettings.CreateTopicOptions => _createTopicOptions;
        CreateSubscriptionOptions SubscriptionSettings.CreateSubscriptionOptions => _subscriptionConfigurator.GetCreateSubscriptionOptions();

        public CreateRuleOptions Rule { get; set; }
        public RuleFilter Filter { get; set; }

        public override string Path { get; }

        public bool RemoveSubscriptions { get; set; }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (_subscriptionConfigurator.AutoDeleteOnIdle.HasValue && _subscriptionConfigurator.AutoDeleteOnIdle.Value > TimeSpan.Zero
                && _subscriptionConfigurator.AutoDeleteOnIdle.Value != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={_subscriptionConfigurator.AutoDeleteOnIdle.Value.TotalSeconds}";
        }

        public override void SelectBasicTier()
        {
            _subscriptionConfigurator.AutoDeleteOnIdle = default;
            _subscriptionConfigurator.DefaultMessageTimeToLive = Defaults.BasicMessageTimeToLive;
        }
    }
}
