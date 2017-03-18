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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using MassTransit.Topology;
    using Newtonsoft.Json.Linq;
    using Specifications;
    using Util;


    public class RabbitMqMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IRabbitMqMessageConsumeTopologyConfigurator<TMessage>,
        IRabbitMqMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IList<IRabbitMqConsumeTopologySpecification> _specifications;

        public RabbitMqMessageConsumeTopology(IMessageEntityNameFormatter<TMessage> entityNameFormatter,
            IMessageExchangeTypeSelector<TMessage> exchangeTypeSelector)
            : base(entityNameFormatter)
        {
            ExchangeTypeSelector = exchangeTypeSelector;

            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        IMessageExchangeTypeSelector<TMessage> ExchangeTypeSelector { get; }

        bool IsBindableMessageType => typeof(JToken) != typeof(TMessage);

        IRabbitMqMessageConsumeTopologyConfigurator<T> IRabbitMqMessageConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            var result = this as IRabbitMqMessageConsumeTopologyConfigurator<T>;
            if (result == null)
                throw new ArgumentException($"The expected message type was invalid: {TypeMetadataCache<T>.ShortName}");

            return result;
        }

        public void Apply(IRabbitMqConsumeTopologyBuilder builder)
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

            var exchangeName = EntityNameFormatter.FormatEntityName();
            var exchangeType = ExchangeTypeSelector.GetExchangeType(exchangeName);

            var temporary = TypeMetadataCache<TMessage>.IsTemporaryMessageType;

            var autoDelete = temporary;
            var durable = !temporary;

            var binding = new ExchangeBindingConfigurator(exchangeName, exchangeType, durable, autoDelete, "");

            configure?.Invoke(binding);

            var specification = new ExchangeBindingConsumeTopologySpecification(binding);

            _specifications.Add(specification);
        }
    }
}