// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport.Settings
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Microsoft.ServiceBus.Messaging;
    using Topology.Configuration;
    using Topology.Configuration.Configurators;
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
        {
            _topicDescription = topicDescription;
            _subscriptionConfigurator = new SubscriptionConfigurator(topicDescription.Path, subscriptionName);

            Path = string.Join("/", _subscriptionConfigurator.TopicPath, _subscriptionConfigurator.SubscriptionName);

            MaxConcurrentCalls = Math.Max(Environment.ProcessorCount, 8);
            PrefetchCount = Math.Max(MaxConcurrentCalls, 32);
        }

        TopicDescription SubscriptionSettings.TopicDescription => _topicDescription;
        SubscriptionDescription SubscriptionSettings.SubscriptionDescription => _subscriptionConfigurator.GetSubscriptionDescription();

        public override TimeSpan LockDuration => _subscriptionConfigurator.LockDuration ?? Defaults.LockDuration;

        public override string Path { get; }

        public override bool RequiresSession => _subscriptionConfigurator.RequiresSession ?? false;

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (_subscriptionConfigurator.AutoDeleteOnIdle.HasValue && _subscriptionConfigurator.AutoDeleteOnIdle.Value > TimeSpan.Zero
                && _subscriptionConfigurator.AutoDeleteOnIdle.Value != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={_subscriptionConfigurator.AutoDeleteOnIdle.Value.TotalSeconds}";
        }

        public override void SelectBasicTier()
        {
            _subscriptionConfigurator.AutoDeleteOnIdle = default(TimeSpan?);
            _subscriptionConfigurator.DefaultMessageTimeToLive = Defaults.BasicMessageTimeToLive;
        }

        public ISubscriptionConfigurator SubscriptionConfigurator => _subscriptionConfigurator;
    }
}