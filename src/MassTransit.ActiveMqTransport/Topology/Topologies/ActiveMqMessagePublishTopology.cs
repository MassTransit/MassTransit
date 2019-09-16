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
namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Entities;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Util;


    public class ActiveMqMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IActiveMqMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly TopicConfigurator _topic;
        readonly IList<IActiveMqMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;

        public ActiveMqMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            var topicName = $"VirtualTopic.{messageTopology.EntityName}";

            var temporary = TypeMetadataCache<TMessage>.IsTemporaryMessageType;

            var durable = !temporary;
            var autoDelete = temporary;

            _topic = new TopicConfigurator(topicName, durable, autoDelete);

            _implementedMessageTypes = new List<IActiveMqMessagePublishTopology>();
        }

        public Topic Topic => _topic;

        bool ITopicConfigurator.Durable
        {
            set => _topic.Durable = value;
        }

        bool ITopicConfigurator.AutoDelete
        {
            set => _topic.AutoDelete = value;
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
        {
            publishAddress = GetSendSettings().GetSendAddress(baseAddress);
            return true;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.CreateTopic(_topic.EntityName, _topic.Durable, _topic.AutoDelete);

            if (builder.Topic != null)
                //                builder.ExchangeBind(builder.Exchange, exchangeHandle, "", new Dictionary<string, object>());
                //            else
                //                builder.Exchange = exchangeHandle;

                foreach (IActiveMqMessagePublishTopology configurator in _implementedMessageTypes)
                    configurator.Apply(builder);
        }

        public SendSettings GetSendSettings()
        {
            return new TopicSendSettings(_topic.EntityName, _topic.Durable, _topic.AutoDelete);
        }

        public BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options)
        {
            var builder = new PublishEndpointBrokerTopologyBuilder(options);

            Apply(builder);

            return builder.BuildBrokerTopology();
        }

        public void AddImplementedMessageConfigurator<T>(IActiveMqMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            IActiveMqMessagePublishTopology
            where T : class
        {
            readonly IActiveMqMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public ImplementedTypeAdapter(IActiveMqMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
            {
                if (_direct)
                {
                    var implementedBuilder = builder.CreateImplementedBuilder();

                    _configurator.Apply(implementedBuilder);
                }
            }
        }
    }
}