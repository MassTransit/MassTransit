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
namespace MassTransit.AmazonSqsTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueSubscriptionEntity :
        QueueSubscription,
        QueueSubscriptionHandle
    {
        readonly QueueEntity _queue;
        readonly TopicEntity _topic;

        public QueueSubscriptionEntity(long id, TopicEntity topic, QueueEntity queue)
        {
            Id = id;
            _topic = topic;
            _queue = queue;
        }

        public static IEqualityComparer<QueueSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<QueueSubscriptionEntity> EntityComparer { get; } = new ConsumerEntityEqualityComparer();

        public Topic Source => _topic.Topic;
        public Queue Destination => _queue.Queue;

        public long Id { get; }
        public QueueSubscription QueueSubscription => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"source: {Source.EntityName}",
                $"destination: {Destination.EntityName}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class ConsumerEntityEqualityComparer : IEqualityComparer<QueueSubscriptionEntity>
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

                return x._topic.Equals(y._topic) && x._queue.Equals(y._queue);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._topic.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._queue.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer : IEqualityComparer<QueueSubscriptionEntity>
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

                return string.Equals(x._topic.EntityName, y._topic.EntityName);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                return obj._topic.EntityName.GetHashCode();
            }
        }
    }
}