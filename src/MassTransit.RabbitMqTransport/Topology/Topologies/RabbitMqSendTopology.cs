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
    using System.Net;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Topologies;
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

        public Uri GetErrorAddress(QueueConfigurator configurator, Uri hostAddress)
        {
            return GetQueueAddress(configurator, hostAddress, "_error");
        }

        public Uri GetDeadLetterAddress(QueueConfigurator configurator, Uri hostAddress)
        {
            return GetQueueAddress(configurator, hostAddress, "_skipped");
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

        public BrokerTopology GetBrokerTopology(Uri address)
        {
            var settings = GetSendSettings(address);

            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.Exchange = builder.ExchangeDeclare(settings.ExchangeName, settings.ExchangeType, settings.Durable, settings.AutoDelete,
                settings.ExchangeArguments);

            if (settings.BindToQueue)
            {
                var queue = builder.QueueDeclare(settings.QueueName, settings.Durable, settings.AutoDelete, false, settings.QueueArguments);
                builder.QueueBind(builder.Exchange, queue, "", new Dictionary<string, object>());
            }

            foreach (var specification in settings.PublishTopologySpecifications)
            {
                specification.Apply(builder);
            }

            return builder.BuildTopologyLayout();
        }

        protected override IMessageSendTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new RabbitMqMessageSendTopology<T>();

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }

        Uri GetQueueAddress(QueueConfigurator configurator, Uri hostAddress, string suffix)
        {
            var queueName = configurator.QueueName + suffix;
            var sendSettings = new RabbitMqSendSettings(queueName, ExchangeTypeSelector.DefaultExchangeType, configurator.Durable, configurator.AutoDelete);

            sendSettings.BindToQueue(queueName);

            return sendSettings.GetSendAddress(hostAddress);
        }
    }
}