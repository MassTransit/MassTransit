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
    using System.Net;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Settings;
    using Util;


    public class RabbitMqSendTopology :
        SendTopology,
        IRabbitMqSendTopologyConfigurator
    {
        public RabbitMqSendTopology(IEntityNameValidator validator)
        {
            ExchangeTypeSelector = new FanoutExchangeTypeSelector();
            EntityNameValidator = validator;
        }

        public IExchangeTypeSelector ExchangeTypeSelector { get; }
        public IEntityNameValidator EntityNameValidator { get; }

        IRabbitMqMessageSendTopologyConfigurator<T> IRabbitMqSendTopology.GetMessageTopology<T>()
        {
            IMessageSendTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IRabbitMqMessageSendTopologyConfigurator<T>;
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
                name = pathSegments[1];

            if (name == "*")
                throw new ArgumentException("Cannot send to a dynamic address");

            EntityNameValidator.ThrowIfInvalidEntityName(name);

            var isTemporary = address.Query.GetValueFromQueryString("temporary", false);

            var durable = address.Query.GetValueFromQueryString("durable", !isTemporary);
            var autoDelete = address.Query.GetValueFromQueryString("autodelete", isTemporary);

            var exchangeType = address.Query.GetValueFromQueryString("type") ?? ExchangeTypeSelector.DefaultExchangeType;

            var settings = new RabbitMqSendSettings(name, exchangeType, durable, autoDelete);

            var bindToQueue = address.Query.GetValueFromQueryString("bind", false);
            if (bindToQueue)
            {
                var queueName = WebUtility.UrlDecode(address.Query.GetValueFromQueryString("queue"));
                settings.BindToQueue(queueName);
            }

            var delayedType = address.Query.GetValueFromQueryString("delayedType");
            if (!string.IsNullOrWhiteSpace(delayedType))
                settings.SetExchangeArgument("x-delayed-type", delayedType);

            var bindExchange = address.Query.GetValueFromQueryString("bindexchange");
            if (!string.IsNullOrWhiteSpace(bindExchange))
                settings.BindToExchange(bindExchange);

            return settings;
        }

        public ErrorSettings GetErrorSettings(EntitySettings settings)
        {
            return new RabbitMqErrorSettings(settings, settings.ExchangeName + "_error");
        }

        public DeadLetterSettings GetDeadLetterSettings(EntitySettings settings)
        {
            return new RabbitMqDeadLetterSettings(settings, settings.ExchangeName + "_skipped");
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new RabbitMqMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}