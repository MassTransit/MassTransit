namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Topology;
    using Topology.Configurators;
    using Transport;


    public class SubscriptionEndpointSettings :
        BaseClientSettings,
        SubscriptionSettings
    {
        readonly SubscriptionConfigurator _subscriptionConfigurator;
        readonly TopicDescription _topicDescription;

        public SubscriptionEndpointSettings(string topicName, string subscriptionName)
            : this(Defaults.CreateTopicDescription(topicName), subscriptionName)
        {
        }

        public SubscriptionEndpointSettings(TopicDescription topicDescription, string subscriptionName)
            : this(topicDescription, new SubscriptionConfigurator(topicDescription.Path, subscriptionName))
        {
        }

        SubscriptionEndpointSettings(TopicDescription topicDescription, SubscriptionConfigurator configurator)
            : base(configurator)
        {
            _topicDescription = topicDescription;
            _subscriptionConfigurator = configurator;

            Name = Path = EntityNameFormatter.FormatSubscriptionPath(_subscriptionConfigurator.TopicPath, _subscriptionConfigurator.SubscriptionName);
        }

        public ISubscriptionConfigurator SubscriptionConfigurator => _subscriptionConfigurator;

        TopicDescription SubscriptionSettings.TopicDescription => _topicDescription;
        SubscriptionDescription SubscriptionSettings.SubscriptionDescription => _subscriptionConfigurator.GetSubscriptionDescription();

        public RuleDescription Rule { get; set; }
        public Filter Filter { get; set; }

        public override TimeSpan LockDuration => _subscriptionConfigurator.LockDuration ?? Defaults.LockDuration;

        public override string Path { get; }

        public override bool RequiresSession => _subscriptionConfigurator.RequiresSession ?? false;

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
