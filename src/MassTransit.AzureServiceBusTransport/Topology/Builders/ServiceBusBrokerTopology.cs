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
namespace MassTransit.AzureServiceBusTransport.Topology.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using GreenPipes;


    public class ServiceBusBrokerTopology :
        BrokerTopology
    {
        public ServiceBusBrokerTopology(IEnumerable<Topic> topics, IEnumerable<Subscription> subscriptions, IEnumerable<Queue> queues,
            IEnumerable<QueueSubscription> queueSubscriptions, IEnumerable<TopicSubscription> topicSubscriptions)
        {
            Topics = topics.ToArray();
            Queues = queues.ToArray();
            Subscriptions = subscriptions.ToArray();
            QueueSubscriptions = queueSubscriptions.ToArray();
            TopicSubscriptions = topicSubscriptions.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public Subscription[] Subscriptions { get; }
        public QueueSubscription[] QueueSubscriptions { get; }
        public TopicSubscription[] TopicSubscriptions { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var topic in Topics)
            {
                var exchangeScope = context.CreateScope("topic");
                exchangeScope.Set(topic.TopicDescription);
            }

            foreach (var queue in Queues)
            {
                var exchangeScope = context.CreateScope("queue");
                exchangeScope.Set(queue.QueueDescription);
            }
            foreach (var subscription in Subscriptions)
            {
                var subscriptionScope = context.CreateScope("subscription");
                subscriptionScope.Set(subscription.SubscriptionDescription);
            }

            foreach (var queueSubscription in QueueSubscriptions)
            {
                var queueSubscriptionScope = context.CreateScope("queueSubscription");
                queueSubscriptionScope.Set(queueSubscription.Subscription.SubscriptionDescription);
            }

            foreach (var topicSubscription in TopicSubscriptions)
            {
                var topicSubscriptionScope = context.CreateScope("topicSubscription");
                topicSubscriptionScope.Set(topicSubscription.Subscription.SubscriptionDescription);
            }
        }
    }
}