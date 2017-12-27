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
    using System.Text;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using Configuration.Specifications;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;
    using NewIdFormatters;
    using Util;


    public class RabbitMqConsumeTopology :
        ConsumeTopology,
        IRabbitMqConsumeTopologyConfigurator
    {
        static readonly INewIdFormatter _formatter = new ZBase32Formatter();
        readonly IMessageTopology _messageTopology;
        readonly IRabbitMqPublishTopology _publishTopology;
        readonly IList<IRabbitMqConsumeTopologySpecification> _specifications;

        public RabbitMqConsumeTopology(IMessageTopology messageTopology, IRabbitMqPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            ExchangeTypeSelector = new FanoutExchangeTypeSelector();

            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        public IExchangeTypeSelector ExchangeTypeSelector { get; }

        IRabbitMqMessageConsumeTopology<T> IRabbitMqConsumeTopology.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IRabbitMqMessageConsumeTopologyConfigurator<T>;
        }

        IRabbitMqMessageConsumeTopologyConfigurator<T> IRabbitMqConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IRabbitMqMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IRabbitMqMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string exchangeName, Action<IExchangeBindingConfigurator> configure = null)
        {
            var exchangeType = ExchangeTypeSelector.DefaultExchangeType;

            var binding = new ExchangeBindingConfigurator(exchangeName, exchangeType, true, false, "");

            configure?.Invoke(binding);

            var specification = new ExchangeBindingConsumeTopologySpecification(binding);

            _specifications.Add(specification);
        }

        public string CreateTemporaryQueueName(string prefix)
        {
            var sb = new StringBuilder(prefix);

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            sb.Append('-');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            sb.Append('-');
            sb.Append(NewId.Next().ToString(_formatter));

            return sb.ToString();
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var exchangeTypeSelector = new MessageExchangeTypeSelector<T>(ExchangeTypeSelector);

            var messageTopology = new RabbitMqMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), exchangeTypeSelector,
                _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}