// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Microsoft.ServiceBus.Messaging;
    using Transport;


    public class SubscriptionEndpointSettings :
        BaseClientSettings,
        SubscriptionSettings
    {
        public SubscriptionEndpointSettings(string topicName, string subscriptionName)
        {
            TopicDescription = Defaults.CreateTopicDescription(topicName);
            SubscriptionDescription = Defaults.CreateSubscriptionDescription(topicName, subscriptionName);
            Path = string.Join("/", SubscriptionDescription.TopicPath, SubscriptionDescription.Name);

            MaxConcurrentCalls = Math.Max(Environment.ProcessorCount, 8);
            PrefetchCount = Math.Max(MaxConcurrentCalls, 32);

            AutoRenewTimeout = TimeSpan.FromSeconds(60);
            MessageWaitTimeout = TimeSpan.FromDays(1);
        }

        public override TimeSpan AutoDeleteOnIdle
        {
            get { return SubscriptionDescription.AutoDeleteOnIdle; }
            set { SubscriptionDescription.AutoDeleteOnIdle = value; }
        }

        public override TimeSpan DefaultMessageTimeToLive
        {
            get { return SubscriptionDescription.DefaultMessageTimeToLive; }
            set { SubscriptionDescription.DefaultMessageTimeToLive = value; }
        }

        public override bool EnableBatchedOperations
        {
            set { SubscriptionDescription.EnableBatchedOperations = value; }
        }

        public override bool EnableDeadLetteringOnMessageExpiration
        {
            set { SubscriptionDescription.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public override string ForwardDeadLetteredMessagesTo
        {
            set { SubscriptionDescription.ForwardDeadLetteredMessagesTo = value; }
        }

        public override int MaxDeliveryCount
        {
            get { return SubscriptionDescription.MaxDeliveryCount; }
            set { SubscriptionDescription.MaxDeliveryCount = value; }
        }

        public override bool RequiresSession
        {
            get { return SubscriptionDescription.RequiresSession; }
            set { SubscriptionDescription.RequiresSession = value; }
        }

        public override string UserMetadata
        {
            set { SubscriptionDescription.UserMetadata = value; }
        }

        public TopicDescription TopicDescription { get; }

        public SubscriptionDescription SubscriptionDescription { get; }

        public override TimeSpan LockDuration
        {
            get { return SubscriptionDescription.LockDuration; }
            set { SubscriptionDescription.LockDuration = value; }
        }

        public override string Path { get; }

        protected override IEnumerable<string> GetQueryStringOptions()
        {
            if (SubscriptionDescription.AutoDeleteOnIdle > TimeSpan.Zero && SubscriptionDescription.AutoDeleteOnIdle != Defaults.AutoDeleteOnIdle)
                yield return $"autodelete={SubscriptionDescription.AutoDeleteOnIdle.TotalSeconds}";
        }

        public override void SelectBasicTier()
        {
            
        }
    }
}