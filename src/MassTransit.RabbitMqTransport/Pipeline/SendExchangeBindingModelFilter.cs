// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Topology;


    /// <summary>
    /// Added to the model pipeline prior to the basic consumer to ensure subscriptions are bound to the consumer properly.
    /// If an subscription fails to declare or bind an exchange or queue, the consumer will not be reached
    /// </summary>
    public class SendExchangeBindingModelFilter :
        IFilter<ModelContext>
    {
        readonly ExchangeBindingSettings _exchangeBindingSettings;

        public SendExchangeBindingModelFilter(ExchangeBindingSettings exchangeBindingSettings)
        {
            _exchangeBindingSettings = exchangeBindingSettings;
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            SendSettings sendSettings;
            if (!context.TryGetPayload(out sendSettings))
                throw new PayloadNotFoundException("The SendSettings were not found.");

            ExchangeSettings exchange = _exchangeBindingSettings.Exchange;

            await context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete,
                exchange.Arguments);

            await context.ExchangeBind(exchange.ExchangeName, sendSettings.ExchangeName, _exchangeBindingSettings.RoutingKey, new Dictionary<string, object>());

            await next.Send(context).ConfigureAwait(false);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}