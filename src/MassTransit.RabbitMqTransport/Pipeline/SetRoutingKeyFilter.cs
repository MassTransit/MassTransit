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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Topology;


    public class SetRoutingKeyFilter<T> :
        IFilter<RabbitMqSendContext<T>>
        where T : class
    {
        readonly IMessageRoutingKeyFormatter<T> _routingKeyFormatter;

        public SetRoutingKeyFilter(IMessageRoutingKeyFormatter<T> routingKeyFormatter)
        {
            _routingKeyFormatter = routingKeyFormatter;
        }

        public Task Send(RabbitMqSendContext<T> context, IPipe<RabbitMqSendContext<T>> next)
        {
            var routingKey = _routingKeyFormatter.FormatRoutingKey(context);

            context.RoutingKey = routingKey;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("SetCorrelationId");
        }
    }
}