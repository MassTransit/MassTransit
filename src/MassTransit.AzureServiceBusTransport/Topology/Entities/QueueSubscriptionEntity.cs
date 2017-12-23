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
namespace MassTransit.AzureServiceBusTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceBus.Messaging;


    public class QueueSubscriptionEntity :
        QueueSubscription,
        QueueSubscriptionHandle
    {
        readonly QueueEntity _queue;
        readonly SubscriptionEntity _subscription;
        readonly TopicEntity _topic;

        public QueueSubscriptionEntity(long id, long subscriptionId, TopicEntity topic, QueueEntity queue, SubscriptionDescription subscriptionDescription)
        {
            Id = id;

            _topic = topic;
            _queue = queue;
            _subscription = new SubscriptionEntity(subscriptionId, topic, subscriptionDescription);
        }

        public static IEqualityComparer<QueueSubscriptionEntity> EntityComparer { get; } = new QueueSubscriptionEntityEqualityComparer();
        public static IEqualityComparer<QueueSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();

        public Topic Source => _topic.Topic;
        public Queue Destination => _queue.Queue;
        public Subscription Subscription => _subscription;

        public long Id { get; }
        public QueueSubscription QueueSubscription => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"topic: {_topic.TopicDescription.Path}",
                $"queue: {_queue.QueueDescription.Path}",
                $"subscription: {_subscription.SubscriptionDescription.Name}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueSubscriptionEntityEqualityComparer :
            IEqualityComparer<QueueSubscriptionEntity>
        {
            public bool Equals(QueueSubscriptionEntity x, QueueSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return TopicEntity.EntityComparer.Equals(x._topic, y._topic)
                    && QueueEntity.EntityComparer.Equals(x._queue, y._queue)
                    && SubscriptionEntity.EntityComparer.Equals(x._subscription, y._subscription);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = TopicEntity.EntityComparer.GetHashCode(obj._topic);
                    hashCode = (hashCode * 397) ^ QueueEntity.EntityComparer.GetHashCode(obj._queue);
                    hashCode = (hashCode * 397) ^ SubscriptionEntity.EntityComparer.GetHashCode(obj._subscription);

                    return hashCode;
                }
            }
        }

        sealed class NameEqualityComparer :
            IEqualityComparer<QueueSubscriptionEntity>
        {
            public bool Equals(QueueSubscriptionEntity x, QueueSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.Subscription.SubscriptionDescription.Name, y.Subscription.SubscriptionDescription.Name)
                    && string.Equals(x.Subscription.SubscriptionDescription.TopicPath, y.Subscription.SubscriptionDescription.TopicPath)
                    && string.Equals(x.Destination.QueueDescription.Path, y.Destination.QueueDescription.Path);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                var hashCode = obj.Subscription.SubscriptionDescription.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Subscription.SubscriptionDescription.TopicPath.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Destination.QueueDescription.Path.GetHashCode();

                return hashCode;
            }
        }

    }
}