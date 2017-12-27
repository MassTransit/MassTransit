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
namespace MassTransit.RabbitMqTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using Configuration.Specifications;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Newtonsoft.Json.Linq;
    using Util;


    public class RabbitMqMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IRabbitMqMessageConsumeTopologyConfigurator<TMessage>,
        IRabbitMqMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IRabbitMqMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IRabbitMqConsumeTopologySpecification> _specifications;

        public RabbitMqMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IMessageExchangeTypeSelector<TMessage> exchangeTypeSelector,
            IRabbitMqMessagePublishTopology<TMessage> publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            ExchangeTypeSelector = exchangeTypeSelector;

            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        IMessageExchangeTypeSelector<TMessage> ExchangeTypeSelector { get; }

        bool IsBindableMessageType => typeof(JToken) != typeof(TMessage);

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
            {
                specification.Apply(builder);
            }
        }

        public void Bind(Action<IExchangeBindingConfigurator> configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidRabbitMqConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var binding = new ExchangeBindingConfigurator(_publishTopology.Exchange, "");

            configure?.Invoke(binding);

            var specification = new ExchangeBindingConsumeTopologySpecification(binding);

            _specifications.Add(specification);
        }
    }
}