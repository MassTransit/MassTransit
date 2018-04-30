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
namespace MassTransit.RabbitMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using GreenPipes;


    /// <summary>
    /// Used to declare an exchange and queue, and bind them together.
    /// </summary>
    public class ExchangeToQueueBindingPublishTopologySpecification :
        QueueBindingConfigurator,
        IRabbitMqPublishTopologySpecification
    {
        public ExchangeToQueueBindingPublishTopologySpecification(string exchangeName, string exchangeType, string queueName = null, bool durable = true,
            bool autoDelete = false)
            : base(queueName ?? exchangeName, exchangeType, durable, autoDelete)
        {
            ExchangeName = exchangeName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            var queueHandle = builder.QueueDeclare(QueueName, Durable, AutoDelete, Exclusive, QueueArguments);

            var bindingHandle = builder.QueueBind(exchangeHandle, queueHandle, RoutingKey, BindingArguments);
        }
    }
}