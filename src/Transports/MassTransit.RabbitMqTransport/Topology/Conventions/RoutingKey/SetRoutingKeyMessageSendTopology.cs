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
namespace MassTransit.RabbitMqTransport.Topology.Conventions.RoutingKey
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Topology;
    using Pipeline;


    public class SetRoutingKeyMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetRoutingKeyMessageSendTopology(IMessageRoutingKeyFormatter<T> routingKeyFormatter)
        {
            if (routingKeyFormatter == null)
                throw new ArgumentNullException(nameof(routingKeyFormatter));

            _filter = new Proxy(new SetRoutingKeyFilter<T>(routingKeyFormatter));
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }


        class Proxy :
            IFilter<SendContext<T>>
        {
            readonly IFilter<RabbitMqSendContext<T>> _filter;

            public Proxy(IFilter<RabbitMqSendContext<T>> filter)
            {
                _filter = filter;
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                var rabbitMqSendContext = context.GetPayload<RabbitMqSendContext<T>>();

                return _filter.Send(rabbitMqSendContext, next);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}