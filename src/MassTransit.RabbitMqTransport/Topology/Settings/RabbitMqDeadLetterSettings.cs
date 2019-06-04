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
namespace MassTransit.RabbitMqTransport.Topology.Settings
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using MassTransit.RabbitMqTransport.Topology.Entities;

    public class RabbitMqDeadLetterSettings :
        QueueBindingConfigurator,
        DeadLetterSettings
    {

        public bool EnableQueue { get; }

        public bool EnableExchange { get; }

        public RabbitMqDeadLetterSettings(EntitySettings source, string name)
            : base(name, source.ExchangeName, source.ExchangeType, source.Durable, source.AutoDelete)
        {
            QueueName = name;
            EnableQueue = source.EnableQueue;
            EnableExchange = source.EnableExchange;

            if (EnableExchange)
            {
                foreach (KeyValuePair<string, object> argument in source.ExchangeArguments)
                    SetExchangeArgument(argument.Key, argument.Value);
            }
        }
        
        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();
            
            QueueHandle queue = null;

            if (EnableQueue)
            {
                queue = builder.QueueDeclare(QueueName, Durable, AutoDelete, false, QueueArguments);
            }

            if (EnableExchange)
            {
                builder.Exchange = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

                if (EnableQueue)
                {
                    builder.QueueBind(builder.Exchange, queue, RoutingKey, BindingArguments);
                }
            }

            return builder.BuildBrokerTopology();
        }
    }
}
