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
namespace MassTransit.ActiveMqTransport.Topology.Builders
{
    using System.Threading;
    using Entities;
    using MassTransit.Topology.Entities;


    public abstract class BrokerTopologyBuilder
    {
        long _nextId;
        protected NamedEntityCollection<ConsumerEntity, ConsumerHandle> ConsumerBindings;
        protected NamedEntityCollection<TopicEntity, TopicHandle> Exchanges;
        protected NamedEntityCollection<QueueEntity, QueueHandle> Queues;

        protected BrokerTopologyBuilder()
        {
            Exchanges = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            ConsumerBindings = new NamedEntityCollection<ConsumerEntity, ConsumerHandle>(ConsumerEntity.EntityComparer, ConsumerEntity.NameComparer);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        public TopicHandle CreateTopic(string name, bool durable, bool autoDelete)
        {
            var id = GetNextId();

            var exchange = new TopicEntity(id, name, durable, autoDelete);

            return Exchanges.GetOrAdd(exchange);
        }

        public QueueHandle CreateQueue(string name, bool durable, bool autoDelete)
        {
            var id = GetNextId();

            var queue = new QueueEntity(id, name, durable, autoDelete);

            return Queues.GetOrAdd(queue);
        }

        public ConsumerHandle BindConsumer(TopicHandle topic, QueueHandle queue, string selector)
        {
            var id = GetNextId();

            var exchangeEntity = Exchanges.Get(topic);

            var queueEntity = Queues.Get(queue);

            var binding = new ConsumerEntity(id, exchangeEntity, queueEntity, selector);

            return ConsumerBindings.GetOrAdd(binding);
        }
    }
}