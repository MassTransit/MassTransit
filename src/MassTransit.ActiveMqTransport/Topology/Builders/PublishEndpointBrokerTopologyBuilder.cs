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
            Exchanges = new NamedEntityCollection<TopicEntity, TopicHandle>(TopicEntity.EntityComparer, TopicEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            ExchangeBindings = new EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle>(ExchangeBindingEntity.EntityComparer);
            QueueBindings = new EntityCollection<QueueBindingEntity, QueueBindingHandle>(QueueBindingEntity.EntityComparer);
        }

        /// <summary>
        /// The exchange to which the published message is sent
        /// </summary>
        public TopicHandle Topic { get; set; }

        public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
        {
            if (_options.HasFlag(PublishBrokerTopologyOptions.MaintainHierarchy))
                return new ImplementedBuilder(this, _options);

            return this;
        }

        public BrokerTopology BuildBrokerTopology()
        {
            return new RabbitMqBrokerTopology(Exchanges, ExchangeBindings, Queues, QueueBindings);
        }


        protected class ImplementedBuilder :
            IPublishEndpointBrokerTopologyBuilder
        {
            readonly IPublishEndpointBrokerTopologyBuilder _builder;
            readonly PublishBrokerTopologyOptions _options;
            TopicHandle _topic;

            public ImplementedBuilder(IPublishEndpointBrokerTopologyBuilder builder, PublishBrokerTopologyOptions options)
            {
                _builder = builder;
                _options = options;
            }

            public TopicHandle Topic
            {
                get => _topic;
                set
                {
                    _topic = value;
                    if (_builder.Topic != null)
                        _builder.BindTopic(_builder.Topic, _topic, "");
                }
            }

            public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
            {
                if (_options.HasFlag(PublishBrokerTopologyOptions.MaintainHierarchy))
                    return new ImplementedBuilder(this, _options);

                return this;
            }

            public TopicHandle CreateTopic(string name, bool durable, bool autoDelete)
            {
                return _builder.CreateTopic(name, durable, autoDelete);
            }

            public ExchangeBindingHandle BindTopic(TopicHandle source, TopicHandle destination, string routingKey)
            {
                return _builder.BindTopic(source, destination, routingKey);
            }

            public QueueHandle CreateQueue(string name, bool durable, bool autoDelete)
            {
                return _builder.CreateQueue(name, durable, autoDelete);
            }

            public QueueBindingHandle BindQueue(TopicHandle topic, QueueHandle queue, string routingKey)
            {
                return _builder.BindQueue(topic, queue, routingKey);
            }
        }
    }
}