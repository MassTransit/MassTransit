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
    using System.Threading;
    using Entities;
    using MassTransit.Topology.Entities;


    public abstract class BrokerTopologyBuilder
    {
        long _nextId;
        protected NamedEntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle> QueueSubscriptions;
        protected NamedEntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle> TopicSubscriptions;
        protected NamedEntityCollection<TopicEntity, TopicHandle> Topics;
        protected NamedEntityCollection<QueueEntity, QueueHandle> Queues;

        protected BrokerTopologyBuilder()
        {
            Topics = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);
            QueueSubscriptions = new NamedEntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle>(QueueSubscriptionEntity.EntityComparer, QueueSubscriptionEntity.NameComparer);
            TopicSubscriptions = new NamedEntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle>(TopicSubscriptionEntity.EntityComparer, TopicSubscriptionEntity.NameComparer);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        public TopicHandle CreateTopic(string name, bool durable, bool autoDelete, IDictionary<string, object> topicAttributes = null, IDictionary<string, object> topicSubscriptionAttributes = null, IDictionary<string, string> tags = null)
        {
            var id = GetNextId();

            var topicEntity = new TopicEntity(id, name, durable, autoDelete, topicAttributes, topicSubscriptionAttributes, tags);

            return Topics.GetOrAdd(topicEntity);
        }

        public QueueHandle CreateQueue(string name, bool durable, bool autoDelete, IDictionary<string, object> queueAttributes = null, IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> tags = null)
        {
            var id = GetNextId();

            var queueEntity = new QueueEntity(id, name, durable, autoDelete, queueAttributes, queueSubscriptionAttributes, tags);

            return Queues.GetOrAdd(queueEntity);
        }

        public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue)
        {
            var id = GetNextId();

            var topicEntity = Topics.Get(topic);

            var queueEntity = Queues.Get(queue);

            var binding = new QueueSubscriptionEntity(id, topicEntity, queueEntity);

            return QueueSubscriptions.GetOrAdd(binding);
        }

        public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination)
        {
            var id = GetNextId();

            var sourceEntity = Topics.Get(source);

            var destinationEntity = Topics.Get(destination);

            var binding = new TopicSubscriptionEntity(id, sourceEntity, destinationEntity);

            return TopicSubscriptions.GetOrAdd(binding);
        }
    }
}
