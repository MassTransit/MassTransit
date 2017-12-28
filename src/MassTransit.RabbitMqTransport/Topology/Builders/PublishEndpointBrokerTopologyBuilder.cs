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
    using Entities;
    using MassTransit.Topology.Entities;


    public class PublishEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IPublishEndpointBrokerTopologyBuilder
    {
        readonly PublishBrokerTopologyOptions _options;

        public PublishEndpointBrokerTopologyBuilder(PublishBrokerTopologyOptions options = PublishBrokerTopologyOptions.FlattenHierarchy)
        {
            _options = options;
            Exchanges = new NamedEntityCollection<ExchangeEntity, ExchangeHandle>(ExchangeEntity.EntityComparer, ExchangeEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            ExchangeBindings = new EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle>(ExchangeBindingEntity.EntityComparer);
            QueueBindings = new EntityCollection<QueueBindingEntity, QueueBindingHandle>(QueueBindingEntity.EntityComparer);
        }

        /// <summary>
        /// The exchange to which the published message is sent
        /// </summary>
        public ExchangeHandle Exchange { get; set; }

        public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
        {
            if (_options.HasFlag(PublishBrokerTopologyOptions.MaintainHierarchy))
            {
                return new ImplementedBuilder(this, _options);
            }

            return this;
        }

        public BrokerTopology BuildTopologyLayout()
        {
            return new RabbitMqBrokerTopology(Exchanges, ExchangeBindings, Queues, QueueBindings);
        }


        protected class ImplementedBuilder :
            IPublishEndpointBrokerTopologyBuilder
        {
            readonly IPublishEndpointBrokerTopologyBuilder _builder;
            readonly PublishBrokerTopologyOptions _options;
            ExchangeHandle _exchange;

            public ImplementedBuilder(IPublishEndpointBrokerTopologyBuilder builder, PublishBrokerTopologyOptions options)
            {
                _builder = builder;
                _options = options;
            }

            public ExchangeHandle Exchange
            {
                get { return _exchange; }
                set
                {
                    _exchange = value;
                    if (_builder.Exchange != null)
                    {
                        _builder.ExchangeBind(_builder.Exchange, _exchange, "", new Dictionary<string, object>());
                    }
                }
            }

            public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
            {
                if (_options.HasFlag(PublishBrokerTopologyOptions.MaintainHierarchy))
                {
                    return new ImplementedBuilder(this, _options);
                }

                return this;
            }

            public ExchangeHandle ExchangeDeclare(string name, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
            {
                return _builder.ExchangeDeclare(name, type, durable, autoDelete, arguments);
            }

            public ExchangeBindingHandle ExchangeBind(ExchangeHandle source, ExchangeHandle destination, string routingKey,
                IDictionary<string, object> arguments)
            {
                return _builder.ExchangeBind(source, destination, routingKey, arguments);
            }

            public QueueHandle QueueDeclare(string name, bool durable, bool autoDelete, bool exclusive, IDictionary<string, object> arguments)
            {
                return _builder.QueueDeclare(name, durable, autoDelete, exclusive, arguments);
            }

            public QueueBindingHandle QueueBind(ExchangeHandle exchange, QueueHandle queue, string routingKey, IDictionary<string, object> arguments)
            {
                return _builder.QueueBind(exchange, queue, routingKey, arguments);
            }
        }
    }
}