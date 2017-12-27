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
namespace MassTransit.RabbitMqTransport.Topology.Builders
{
    using System.Collections.Generic;
    using System.Threading;
    using Entities;
    using MassTransit.Topology.Entities;


    public abstract class BrokerTopologyBuilder
    {
        long _nextId;
        protected EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle> ExchangeBindings;
        protected NamedEntityCollection<ExchangeEntity, ExchangeHandle> Exchanges;
        protected EntityCollection<QueueBindingEntity, QueueBindingHandle> QueueBindings;
        protected NamedEntityCollection<QueueEntity, QueueHandle> Queues;

        protected BrokerTopologyBuilder()
        {
            Exchanges = new NamedEntityCollection<ExchangeEntity, ExchangeHandle>(ExchangeEntity.EntityComparer, ExchangeEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            ExchangeBindings = new EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle>(ExchangeBindingEntity.EntityComparer);
            QueueBindings = new EntityCollection<QueueBindingEntity, QueueBindingHandle>(QueueBindingEntity.EntityComparer);
        }

        long GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        public ExchangeHandle ExchangeDeclare(string name, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var exchange = new ExchangeEntity(id, name, type, durable, autoDelete, arguments);

            return Exchanges.GetOrAdd(exchange);
        }

        public ExchangeBindingHandle ExchangeBind(ExchangeHandle source, ExchangeHandle destination, string routingKey, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var sourceExchange = Exchanges.Get(source);

            var destinationExchange = Exchanges.Get(destination);

            var binding = new ExchangeBindingEntity(id, sourceExchange, destinationExchange, routingKey, arguments);

            return ExchangeBindings.GetOrAdd(binding);
        }

        public QueueHandle QueueDeclare(string name, bool durable, bool autoDelete, bool exclusive, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var queue = new QueueEntity(id, name, durable, autoDelete, exclusive, arguments);

            return Queues.GetOrAdd(queue);
        }

        public QueueBindingHandle QueueBind(ExchangeHandle exchange, QueueHandle queue, string routingKey, IDictionary<string, object> arguments)
        {
            var id = GetNextId();

            var exchangeEntity = Exchanges.Get(exchange);

            var queueEntity = Queues.Get(queue);

            var binding = new QueueBindingEntity(id, exchangeEntity, queueEntity, routingKey, arguments);

            return QueueBindings.GetOrAdd(binding);
        }
    }
}