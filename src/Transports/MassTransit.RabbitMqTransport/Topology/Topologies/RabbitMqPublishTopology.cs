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
namespace MassTransit.RabbitMqTransport.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;


    public class RabbitMqPublishTopology :
        PublishTopology,
        IRabbitMqPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public RabbitMqPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
            ExchangeTypeSelector = new FanoutExchangeTypeSelector();
        }

        public IExchangeTypeSelector ExchangeTypeSelector { get; }

        public PublishBrokerTopologyOptions BrokerTopologyOptions { get; set; }

        IRabbitMqMessagePublishTopology<T> IRabbitMqPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IRabbitMqMessagePublishTopologyConfigurator<T>;
        }

        IRabbitMqMessagePublishTopologyConfigurator<T> IRabbitMqPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IRabbitMqMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var exchangeTypeSelector = new MessageExchangeTypeSelector<T>(ExchangeTypeSelector);

            var messageTopology = new RabbitMqMessagePublishTopology<T>(this, _messageTopology.GetMessageTopology<T>(), exchangeTypeSelector);

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly RabbitMqMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IRabbitMqPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IRabbitMqPublishTopologyConfigurator publishTopology,
                RabbitMqMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IRabbitMqMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}