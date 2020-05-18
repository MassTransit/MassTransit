// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Topology.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using GreenPipes;


    public class AmazonSqsBrokerTopology :
        BrokerTopology
    {
        public AmazonSqsBrokerTopology(IEnumerable<Topic> exchanges, IEnumerable<Queue> queues, IEnumerable<QueueSubscription> queueSubscriptions,
            IEnumerable<TopicSubscription> topicSubscriptions)
        {
            Topics = exchanges.ToArray();
            Queues = queues.ToArray();
            QueueSubscriptions = queueSubscriptions.ToArray();
            TopicSubscriptions = topicSubscriptions.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public QueueSubscription[] QueueSubscriptions { get; }
        public TopicSubscription[] TopicSubscriptions { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var topic in Topics)
            {
                var topicScope = context.CreateScope("topic");
                topicScope.Set(new
                {
                    Name = topic.EntityName,
                    topic.Durable,
                    topic.AutoDelete
                });
            }

            foreach (var queue in Queues)
            {
                var queueScope = context.CreateScope("queue");
                queueScope.Set(new
                {
                    Name = queue.EntityName,
                    queue.Durable,
                    queue.AutoDelete
                });
            }

            foreach (var subscription in QueueSubscriptions)
            {
                var subscriptionScope = context.CreateScope("queueSubscription");
                subscriptionScope.Set(new
                {
                    Source = subscription.Source.EntityName,
                    Destination = subscription.Destination.EntityName
                });
            }

            foreach (var subscription in TopicSubscriptions)
            {
                var subscriptionScope = context.CreateScope("topicSubscription");
                subscriptionScope.Set(new
                {
                    Source = subscription.Source.EntityName,
                    Destination = subscription.Destination.EntityName
                });
            }
        }
    }
}
