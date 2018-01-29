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
namespace MassTransit.ActiveMqTransport.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueBindingEntity :
        ExchangeToQueueBinding,
        QueueBindingHandle
    {
        readonly QueueEntity _queue;
        readonly TopicEntity _topic;

        public QueueBindingEntity(long id, TopicEntity topic, QueueEntity queue, string routingKey)
        {
            Id = id;
            RoutingKey = routingKey;
            _topic = topic;
            _queue = queue;
        }

        public static IEqualityComparer<QueueBindingEntity> EntityComparer { get; } = new QueueBindingEntityEqualityComparer();

        public Topic Source => _topic.Topic;
        public Queue Destination => _queue.Queue;
        public string RoutingKey { get; }

        public long Id { get; }
        public ExchangeToQueueBinding Binding => this;

        public override string ToString()
        {
            return string.Join(", ", new[]
            {
                $"source: {Source.EntityName}",
                $"destination: {Destination.EntityName}",
                string.IsNullOrWhiteSpace(RoutingKey) ? "" : $"routing-key: {RoutingKey}"
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueBindingEntityEqualityComparer : IEqualityComparer<QueueBindingEntity>
        {
            public bool Equals(QueueBindingEntity x, QueueBindingEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return x._topic.Equals(y._topic) && x._queue.Equals(y._queue) && string.Equals(x.RoutingKey, y.RoutingKey);
            }

            public int GetHashCode(QueueBindingEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._topic.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._queue.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.RoutingKey.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}