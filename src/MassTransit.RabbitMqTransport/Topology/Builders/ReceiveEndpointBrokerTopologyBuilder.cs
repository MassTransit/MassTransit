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
    using Entities;
    using MassTransit.Topology.Entities;


    public class ReceiveEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IReceiveEndpointBrokerTopologyBuilder
    {
        public ReceiveEndpointBrokerTopologyBuilder()
        {
            Exchanges = new NamedEntityCollection<ExchangeEntity, ExchangeHandle>(ExchangeEntity.EntityComparer, ExchangeEntity.NameComparer);
            Queues = new NamedEntityCollection<QueueEntity, QueueHandle>(QueueEntity.QueueComparer, QueueEntity.NameComparer);

            ExchangeBindings = new EntityCollection<ExchangeBindingEntity, ExchangeBindingHandle>(ExchangeBindingEntity.EntityComparer);
            QueueBindings = new EntityCollection<QueueBindingEntity, QueueBindingHandle>(QueueBindingEntity.EntityComparer);
        }

        public QueueHandle Queue { get; set; }

        public ExchangeHandle Exchange { get; set; }

        public BrokerTopology BuildTopologyLayout()
        {
            return new RabbitMqBrokerTopology(Exchanges, ExchangeBindings, Queues, QueueBindings);
        }
    }
}