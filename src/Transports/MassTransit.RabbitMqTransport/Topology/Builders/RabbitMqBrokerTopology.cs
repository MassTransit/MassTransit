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
namespace MassTransit.RabbitMqTransport.Topology.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using GreenPipes;


    public class RabbitMqBrokerTopology :
        BrokerTopology
    {
        public RabbitMqBrokerTopology(IEnumerable<Exchange> exchanges, IEnumerable<ExchangeToExchangeBinding> exchangeBindings, IEnumerable<Queue> queues,
            IEnumerable<ExchangeToQueueBinding> queueBindings)
        {
            Exchanges = exchanges.ToArray();
            Queues = queues.ToArray();
            ExchangeBindings = exchangeBindings.ToArray();
            QueueBindings = queueBindings.ToArray();
        }

        public Exchange[] Exchanges { get; }
        public Queue[] Queues { get; }
        public ExchangeToExchangeBinding[] ExchangeBindings { get; }
        public ExchangeToQueueBinding[] QueueBindings { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var exchange in Exchanges)
            {
                var exchangeScope = context.CreateScope("exchange");
                exchangeScope.Set(new
                {
                    Name = exchange.ExchangeName,
                    Type = exchange.ExchangeType,
                    exchange.Durable,
                    exchange.AutoDelete
                });
                foreach (KeyValuePair<string, object> argument in exchange.ExchangeArguments)
                {
                    var argumentScope = exchangeScope.CreateScope("argument");
                    argumentScope.Add("key", argument.Key);
                    argumentScope.Add("value", argument.Value);
                }
            }

            foreach (var queue in Queues)
            {
                var exchangeScope = context.CreateScope("queue");
                exchangeScope.Set(new
                {
                    Name = queue.QueueName,
                    queue.Durable,
                    queue.AutoDelete,
                    queue.Exclusive
                });
                foreach (KeyValuePair<string, object> argument in queue.QueueArguments)
                {
                    var argumentScope = exchangeScope.CreateScope("argument");
                    argumentScope.Add("key", argument.Key);
                    argumentScope.Add("value", argument.Value);
                }
            }

            foreach (var binding in ExchangeBindings)
            {
                var exchangeScope = context.CreateScope("exchange-binding");
                exchangeScope.Set(new
                {
                    Source = binding.Source.ExchangeName,
                    Destination = binding.Destination.ExchangeName,
                    binding.RoutingKey
                });
                foreach (KeyValuePair<string, object> argument in binding.Arguments)
                {
                    var argumentScope = exchangeScope.CreateScope("argument");
                    argumentScope.Add("key", argument.Key);
                    argumentScope.Add("value", argument.Value);
                }
            }

            foreach (var binding in QueueBindings)
            {
                var exchangeScope = context.CreateScope("queue-binding");
                exchangeScope.Set(new
                {
                    Source = binding.Source.ExchangeName,
                    Destination = binding.Destination.QueueName,
                    binding.RoutingKey
                });
                foreach (KeyValuePair<string, object> argument in binding.Arguments)
                {
                    var argumentScope = exchangeScope.CreateScope("argument");
                    argumentScope.Add("key", argument.Key);
                    argumentScope.Add("value", argument.Value);
                }
            }
        }
    }
}