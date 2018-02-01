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
namespace MassTransit.ActiveMqTransport.Topology.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using GreenPipes;


    public class ActiveMqBrokerTopology :
        BrokerTopology
    {
        public ActiveMqBrokerTopology(IEnumerable<Topic> exchanges, IEnumerable<Queue> queues, IEnumerable<Consumer> consumers)
        {
            Topics = exchanges.ToArray();
            Queues = queues.ToArray();
            Consumers = consumers.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public Consumer[] Consumers { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var exchange in Topics)
            {
                var exchangeScope = context.CreateScope("exchange");
                exchangeScope.Set(new
                {
                    Name = exchange.EntityName,
                    exchange.Durable,
                    exchange.AutoDelete
                });
            }

            foreach (var queue in Queues)
            {
                var exchangeScope = context.CreateScope("queue");
                exchangeScope.Set(new
                {
                    Name = queue.EntityName,
                    queue.Durable,
                    queue.AutoDelete
                });
            }

            foreach (var binding in Consumers)
            {
                var exchangeScope = context.CreateScope("consumer");
                exchangeScope.Set(new
                {
                    Source = binding.Source.EntityName,
                    Destination = binding.Destination.EntityName,
                    RoutingKey = binding.Selector
                });
            }
        }
    }
}