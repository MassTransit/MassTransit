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
    using System.Threading;
    using Entities;
    using MassTransit.Topology.Entities;
    using Microsoft.ServiceBus.Messaging;


    public abstract class BrokerTopologyBuilder :
        IBrokerTopologyBuilder
    {
        long _nextId;

        protected BrokerTopologyBuilder()
        {
            Topics = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.EntityComparer, QueueEntity.NameComparer);

            Subscriptions = new NamedEntityCollection<SubscriptionEntity, SubscriptionHandle>(SubscriptionEntity.EntityComparer,
                SubscriptionEntity.NameComparer);
            QueueSubscriptions = new NamedEntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle>(QueueSubscriptionEntity.EntityComparer,
                QueueSubscriptionEntity.NameComparer);
            TopicSubscriptions = new NamedEntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle>(TopicSubscriptionEntity.EntityComparer,
                TopicSubscriptionEntity.NameComparer);
        }

        protected EntityCollection<SubscriptionEntity, SubscriptionHandle> Subscriptions { get; }
        protected NamedEntityCollection<TopicEntity, TopicHandle> Topics { get; }
        protected EntityCollection<QueueSubscriptionEntity, QueueSubscriptionHandle> QueueSubscriptions { get; }
        protected EntityCollection<TopicSubscriptionEntity, TopicSubscriptionHandle> TopicSubscriptions { get; }
        protected NamedEntityCollection<QueueEntity, QueueHandle> Queues { get; }

        public TopicHandle CreateTopic(TopicDescription topicDescription)
        {
            var exchange = new TopicEntity(GetNextId(), topicDescription);

            return Topics.GetOrAdd(exchange);
        }

        public SubscriptionHandle CreateSubscription(TopicHandle topic, SubscriptionDescription subscriptionDescription)
        {
            var topicEntity = Topics.Get(topic);

            var subscriptionEntity = new SubscriptionEntity(GetNextId(), topicEntity, subscriptionDescription);

            return Subscriptions.GetOrAdd(subscriptionEntity);
        }

        public QueueHandle CreateQueue(QueueDescription queueDescription)
        {
            var queue = new QueueEntity(GetNextId(), queueDescription);

            return Queues.GetOrAdd(queue);
        }

        public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, SubscriptionDescription subscriptionDescription)
        {
            var topicEntity = Topics.Get(exchange);

            var queueEntity = Queues.Get(queue);

            if (topicEntity.TopicDescription.EnablePartitioning)
                queueEntity.QueueDescription.EnablePartitioning = true;

            var binding = new QueueSubscriptionEntity(GetNextId(), GetNextId(), topicEntity, queueEntity, subscriptionDescription);

            return QueueSubscriptions.GetOrAdd(binding);
        }

        public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SubscriptionDescription subscriptionDescription)
        {
            var sourceEntity = Topics.Get(source);

            var destinationEntity = Topics.Get(destination);

            if (sourceEntity.TopicDescription.EnablePartitioning)
                destinationEntity.TopicDescription.EnablePartitioning = true;

            var subscriptionEntity = new TopicSubscriptionEntity(GetNextId(), GetNextId(), sourceEntity, destinationEntity, subscriptionDescription);

            return TopicSubscriptions.GetOrAdd(subscriptionEntity);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }
    }
}