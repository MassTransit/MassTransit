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


    public class RabbitMqDeadLetterSettings :
        QueueBindingConfigurator,
        DeadLetterSettings
    {
        public RabbitMqDeadLetterSettings(EntitySettings source, string name)
            : base(name, source.ExchangeType, source.Durable, source.AutoDelete)
        {
            QueueName = name;

            foreach (KeyValuePair<string, object> argument in source.ExchangeArguments)
                SetExchangeArgument(argument.Key, argument.Value);
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.Exchange = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            var queue = builder.QueueDeclare(QueueName, Durable, AutoDelete, false, QueueArguments);

            builder.QueueBind(builder.Exchange, queue, RoutingKey, BindingArguments);

            return builder.BuildBrokerTopology();
        }
    }
}