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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.ComponentModel;
#if !NETCORE
    using Microsoft.ServiceBus.Messaging;
#else
    using Microsoft.Azure.ServiceBus;
#endif

    [EditorBrowsable(EditorBrowsableState.Never)]
    static class Defaults
    {
        public static QueueDescription CreateQueueDescription(string queueName)
        {
            return new QueueDescription(queueName)
            {
                AutoDeleteOnIdle = AutoDeleteOnIdle,
                DefaultMessageTimeToLive = TimeSpan.FromDays(365 + 1),
                EnableBatchedOperations = true,
                EnableDeadLetteringOnMessageExpiration = true,
                LockDuration = TimeSpan.FromMinutes(5),
                MaxDeliveryCount = 5
            };
        }

        public static TopicDescription CreateTopicDescription(string topicName)
        {
            return new TopicDescription(topicName)
            {
                AutoDeleteOnIdle = AutoDeleteOnIdle,
                DefaultMessageTimeToLive = TimeSpan.FromDays(365 + 1),
                EnableBatchedOperations = true
            };
        }

        public static SubscriptionDescription CreateSubscriptionDescription(string topicPath, string subscriptionName, QueueDescription queue, string queuePath)
        {
            return new SubscriptionDescription(topicPath, subscriptionName)
            {
                AutoDeleteOnIdle = queue.AutoDeleteOnIdle,
                DefaultMessageTimeToLive = queue.DefaultMessageTimeToLive,
                EnableBatchedOperations = queue.EnableBatchedOperations,
                EnableDeadLetteringOnMessageExpiration = queue.EnableDeadLetteringOnMessageExpiration,
                ForwardTo = queuePath,
                LockDuration = queue.LockDuration,
                MaxDeliveryCount = queue.MaxDeliveryCount,
                UserMetadata = queue.UserMetadata
            };
        }

        public static SubscriptionDescription CreateSubscriptionDescription(string topicPath, string subscriptionName)
        {
            var queue = CreateQueueDescription(subscriptionName);

            return new SubscriptionDescription(topicPath, subscriptionName)
            {
                AutoDeleteOnIdle = queue.AutoDeleteOnIdle,
                DefaultMessageTimeToLive = queue.DefaultMessageTimeToLive,
                EnableBatchedOperations = queue.EnableBatchedOperations,
                EnableDeadLetteringOnMessageExpiration = queue.EnableDeadLetteringOnMessageExpiration,
                LockDuration = queue.LockDuration,
                MaxDeliveryCount = queue.MaxDeliveryCount,
                UserMetadata = queue.UserMetadata
            };
        }

        public static TimeSpan AutoDeleteOnIdle => TimeSpan.FromDays(427);
    }
}